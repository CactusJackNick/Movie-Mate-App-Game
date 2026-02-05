using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace.Game
{
    public class GameController : IGameController
    {
        private readonly IGameView _view;
        private readonly IApiService _apiService;
        private readonly IClueFactory _clueFactory;
        private readonly IFeedbackService _feedbackService;
        private readonly HashSet<int> _guessedIds = new();

        private int _currentTier = 0;
        private int _currentPage = 1;
        private int _maxPages = 1;
        private string _currentQuery = "";
        private bool _isLoading;
        private CancellationTokenSource _cts;
        private DetailsSuperlistModel _targetMovie;

        public GameController(IGameView view, IApiService apiService
        , IClueFactory clueFactory,  IFeedbackService feedbackService)
        {
            _view = view;
            _apiService = apiService;
            _clueFactory = clueFactory;
            _feedbackService = feedbackService;
        }

        public void LoadMovies()
        { 
            LoadMovieAsync().Forget();
            StartSearch("");
        }
        
        public void LoadNextPageGuesses()
        {
            if (_isLoading)
            {
                return;
            }
            
            if (_currentPage >= _maxPages) 
            {
                return; 
            }

            _currentPage++;
            LoadGuessMoviesAsync(_currentQuery, _currentPage).Forget();
        }
        
        public void StartSearch(string search)
        {
            _maxPages = int.MaxValue;
            _currentQuery = search;
            _currentPage = 1;
            _isLoading = false;
            
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();

            }
            
            _cts = new CancellationTokenSource();
            SearchWithDebounce(search, _cts.Token).Forget();
        }

        private async UniTaskVoid SearchWithDebounce(string query, CancellationToken token)
        {
            await UniTask.Delay(500, cancellationToken: token);
            
            _currentQuery = query;
            _currentPage = 1;
            _isLoading = false;

            if (string.IsNullOrEmpty(query))
            {
                _view.ClearGuessesItems();
                return;
            }
            
            await LoadGuessMoviesAsync(query, _currentPage);
        }
        
        private void ResetSearchState()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            _currentQuery = "";
            _currentPage = 1;
            _maxPages = 1;
            _isLoading = false;
        }

        private async UniTask LoadMovieAsync()
        {
            LoadingPanel.Instance.Show();
            
            const int startClueIndex = 0;
            _currentTier = 0;
            
            _guessedIds.Clear();
            
            var movie = await GetValidGameMovieAsync();
            if (movie == null)
            {
                return;
            }

            _targetMovie = movie;
            
            var poster = await _apiService.GetMovieImageAsync(movie.PosterPath);
            var backdrop = await _apiService.GetMovieImageAsync(movie.BackdropPath);

            var date = movie.Release_Date;
            var tagline = movie.Tagline;
            
            var directorName = movie.Credits.Crew.Find(x => x.Job == "Director").NameCrew ?? "MISSING";
           
            var genresText = "";
            var genresList = new List<string>();
            foreach (var genre in movie.Genres)
            {
                genresList.Add(genre.Name);
            } 
            
            var actorsText = "";
            var actors = new List<string>();
            foreach (var actor in movie.Credits.Cast)
            {
                if (actor.Acting == "Acting")
                {
                    if (actors.Count >= 3)
                    {
                        continue;
                    }
                    
                    actors.Add(actor.ActorName);
                }
            } 
                
            genresText = string.Join(", ", genresList);
            actorsText = string.Join(", ", actors);
            
            
            var clues = _clueFactory.AssignDataToClues
            (
                movie: movie,
                backdrop: backdrop,
                poster: poster
            );
           
            _view.DisplayClues(clues);
            _view.UnlockClue(startClueIndex);
            
            LoadingPanel.Instance.Hide();
        }
        
        private async UniTask LoadGuessMoviesAsync(string query, int page)
        {
            _isLoading =  true;

            try
            {
                var response = await _apiService.SearchMoviesAsync(query, page);
                if (_currentQuery != query)
                {
                    return;
                }
                
                _maxPages = response.TotalPages;
                var validMovies = new List<MovieData>();

                foreach (var movie in response.Results)
                {
                    if (string.IsNullOrEmpty(movie.backdrop_path) ||
                        string.IsNullOrEmpty(movie.poster_path))
                    {
                        continue;
                    }

                    if (_guessedIds.Contains(movie.Id))
                    {
                        continue;
                    }

                    validMovies.Add(movie);
                }

                if (page == 1)
                {
                    _view.DisplayMovies(validMovies);
                }
                else
                {
                    _view.AddMovies(validMovies);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading movies: {ex.Message} ");
                throw new Exception(ex.Message);
            }
            finally
            {
                _isLoading = false; 
            }
        }
        
        private async UniTask<DetailsSuperlistModel> GetValidGameMovieAsync()
        {
            LoadingPanel.Instance.Show();   
            
            const int maxPageSize = 100;
            var maxAttempts = 10;
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                attempts++;
                var randomPageNumber = Random.Range(1, maxPageSize);
                var listResponse = await _apiService.GetPopularMoviesAsync(randomPageNumber);

                if (listResponse.Results == null || listResponse.Results.Count == 0)
                {
                    continue;
                }
                
                var candidates = ShuffleListRandomly(listResponse.Results);

                foreach (var candidate in candidates)
                {
                    if (string.IsNullOrEmpty(candidate.backdrop_path))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(candidate.poster_path))
                    {
                        continue;
                    }

                    if (candidate.Genre_Ids == null || candidate.Genre_Ids.Count == 0)
                    {
                        continue;
                    }

                    var details = await _apiService.GetMovieDetailsAsync(candidate.Id);

                    if (ValidateMovie(details))
                    {
                        Debug.Log($"Found valid game movie: {details.Title}");
                        return details;
                    }
                }
            }

            LoadingPanel.Instance.Hide();
            return null;
        }

        private bool ValidateMovie(DetailsSuperlistModel movie)
        {
            if (string.IsNullOrEmpty(movie.Tagline))
            {
                return false;
            }

            var hasDirector = false;
            foreach (var person in movie.Credits.Crew)
            {
                if (person.Job == "Director")
                {
                    hasDirector = true;
                }
            }
           
            if (!hasDirector)
            {
                return false;
            }

            if (movie.Credits.Cast == null || movie.Credits.Cast.Count < 3)
            {
                return false;
            }
            
            return true;
        }

        private List<MovieData> ShuffleListRandomly(List<MovieData> inputList)
        {    
            //take any list of DetailsSuperlistModel and return it with Fischer-Yates shuffle
            var i = 0;
            var t = inputList.Count;
            MovieData p;
            var tempList = new List<MovieData>();
            tempList.AddRange(inputList);
     
            while (i < t)
            {
                var r = Random.Range(i, tempList.Count);
                p = tempList[i];
                tempList[i] = tempList[r];
                tempList[r] = p;
                i++;
            }
     
            return tempList;
        }

        public async UniTask ProcessPlayerGuess(int guessedMovieId)
        {
            ResetSearchState();
            
            _guessedIds.Add(guessedMovieId);
            
            var guess = await _apiService.GetMovieDetailsAsync(guessedMovieId);

            var result = _feedbackService.EvaluateGuess(guess, _targetMovie);

            _view.ShowFeedbackResult(result);
            
            if (guess.Id == _targetMovie.Id)
            {
                //open win panel
            }
            else
            {
                _currentTier++;
                _view.UnlockClue(_currentTier);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}