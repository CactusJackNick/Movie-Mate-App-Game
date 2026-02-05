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
        private readonly HashSet<int> _guessedIds = new();

        private int _currentTier = 0;
        private int _currentPage = 1;
        private int _maxPages = 1;
        private string _currentQuery = "";
        private bool _isLoading;
        private CancellationTokenSource _cts;
        private DetailsSuperlistModel _targetMovie;

        public GameController(IGameView view, IApiService apiService
        , IClueFactory clueFactory)
        {
            _view = view;
            _apiService = apiService;
            _clueFactory = clueFactory;
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
            
            // var clues = AssignDataToClues
            // (
            //     director: directorName,
            //     date: date,
            //     actors: actorsText,
            //     genres: genresText, 
            //     tagline: tagline,
            //     backdrop: backdrop,
            //     poster: poster
            // );
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

        private List<ClueData> AssignDataToClues(
            string director, string date, string actors, string genres, 
            string tagline, Sprite backdrop, Sprite poster)
        {
            var clues = new List<ClueData>
            {
                new() //clue 1
                {
                    _title = "Backdrop",
                    _displayMode = ClueDisplayMode.Backdrop,
                    _imageContext = backdrop
                },
                new() //clue 2
                {
                    _title = "Year & Genres",
                    _displayMode = ClueDisplayMode.Text, 
                    _textContext = $"{date}\n{genres}"
                },
                new() //clue 3
                {
                    _title = "Director",
                    _displayMode = ClueDisplayMode.Text,
                    _textContext = $"{director}"
                },
                new() //clue 4
                {
                    _title = "Actors",
                    _displayMode = ClueDisplayMode.Text,
                    _textContext = $"{actors}"
                },
                new() //clue 5
                {
                    _title = "Quote",
                    _displayMode = ClueDisplayMode.Text,
                    _textContext = $"{tagline}"
                },
                new() //clue 6
                {
                    _title = "Poster",
                    _displayMode = ClueDisplayMode.Poster,
                    _imageContext = poster
                }
            };

            return clues;
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

            var result = new GuessResultModel
            {
                DirectorName = GetDirectorName(guess),
                DirectorColor = GetDirectorColor(guess, _targetMovie),
                
                ActorsText = GetActorsText(guess),
                ActorsColor = GetActorsColor(guess, _targetMovie),
                
                GenresText = GetGenresText(guess),
                GenresColor = GetGenresColor(guess, _targetMovie),
                
                YearText = GetYearText(guess),
                YearColor = GetYearColor(guess, _targetMovie),
                RotateYearArrow = CompareYearsToDetermineArrowRot(guess, _targetMovie)
            };

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

        private string GetDirectorName(DetailsSuperlistModel model)
        {
            var directorName = "";
            foreach (var person in model.Credits.Crew)
            {
                if (person.Job == "Director")
                {
                   directorName = person.NameCrew;
                }
            }
            return string.IsNullOrEmpty(directorName)
                ? "N/A" 
                : directorName;
        }

        private FeedbackColor GetDirectorColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessDir = GetDirectorName(guess);
            var targetDir = GetDirectorName(target);

            return guessDir == targetDir
                ? FeedbackColor.Green 
                : FeedbackColor.Red;
        }

        private string GetActorsText(DetailsSuperlistModel movie)
        {
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
            
            return string.Join("\n ", actors);
        }

        private FeedbackColor GetActorsColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessActorIds = GetActorIds(guess);
            var targetActorIds = GetActorIds(target);

            if (guessActorIds.SetEquals(targetActorIds))
            {
                return FeedbackColor.Green;
            }

            if (guessActorIds.Overlaps(targetActorIds))
            {
                return FeedbackColor.Orange;
            }
            
            return FeedbackColor.Red;
        }

        private HashSet<int> GetActorIds(DetailsSuperlistModel movie)
        {
            var actorSet  = new HashSet<int>();
            if (movie.Credits.Cast == null)
            {
                return actorSet;
            }
            
            foreach (var actor in movie.Credits.Cast)
            {
                if (actor.Acting == "Acting")
                {
                    if (actorSet.Count >= 3)
                    {
                        break;
                    }

                    actorSet.Add(actor.ActorId);
                }
            }
            return actorSet;
        }

        private string GetGenresText(DetailsSuperlistModel movie)
        {
            var genres = new List<string>();
            foreach (var genre in movie.Genres)
            {
                genres.Add(genre.Name);
            }

            return genres.Count == 0 
                ? "N/A" 
                : string.Join("\n", genres);
        }

        private FeedbackColor GetGenresColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessGenreId = GetGenreIds(guess);
            var targetGenreId = GetGenreIds(target);

            if (guessGenreId.SetEquals(targetGenreId))
            {
                return FeedbackColor.Green;
            }

            if (guessGenreId.Overlaps(targetGenreId))
            {
                return FeedbackColor.Orange;
            }
            
            return FeedbackColor.Red;
        }

        private HashSet<int> GetGenreIds(DetailsSuperlistModel movie)
        {
            var genresSet = new HashSet<int>();
            
            if (movie.Genres == null)
            {
                return genresSet;
            }

            foreach (var genre in movie.Genres)
            {
                genresSet.Add(genre.Id);
            }
            
            return genresSet;
        }

        private string GetYearText(DetailsSuperlistModel movie)
        {
            const int lengthYearToRead = 4;
            var yearText = movie.Release_Date;

            return yearText[..lengthYearToRead];
        }

        private FeedbackColor GetYearColor(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessYear = GetYearText(guess);
            var targetYear = GetYearText(target);
            
            return guessYear == targetYear 
                ? FeedbackColor.Green 
                : FeedbackColor.Red;
        }

        private bool CompareYearsToDetermineArrowRot(DetailsSuperlistModel guess, DetailsSuperlistModel target)
        {
            var guessYear = int.Parse(GetYearText(guess));
            var targetYear = int.Parse(GetYearText(target));

            return guessYear < targetYear;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}