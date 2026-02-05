using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class GameController : IGameController
    {
        private readonly IGameView _view;
        private readonly IApiService _apiService;
        private readonly IClueFactory _clueFactory;
        private readonly IFeedbackService _feedbackService;
        private readonly ISearchController _searchController;
        private readonly IMoviePickService _moviePickService;
        private readonly HashSet<int> _guessedIds = new();

        private DetailsSuperlistModel _targetMovie;
        private int _currentTier = 0;

        public GameController(IGameView view, IApiService apiService
        , IClueFactory clueFactory, IFeedbackService feedbackService,
        ISearchController searchController, IMoviePickService moviePickService)
        {
            _view = view;
            _apiService = apiService;
            _clueFactory = clueFactory;
            _feedbackService = feedbackService;
            _searchController = searchController;
            _moviePickService = moviePickService;
            
            searchController.SetFilter(_guessedIds);
        }

        public void LoadMovies()
        { 
            _currentTier = 0;
            _guessedIds.Clear();
            _searchController.ResetSearchState();
            
            LoadTargetMovieAsync().Forget();
        }
        
        public void StartSearch(string search)
        {
            _searchController.StartSearch(search);
        }
        
        public void LoadNextPageGuesses()
        {
           _searchController.LoadNextPage();
        }
        
        public async UniTask ProcessPlayerGuess(int guessedMovieId)
        {
            _searchController.ResetSearchState();
            
            _guessedIds.Add(guessedMovieId);
            
            var guess = await _apiService.GetMovieDetailsAsync(guessedMovieId);

            var result = _feedbackService.EvaluateGuess(guess, _targetMovie);

            _view.ShowFeedbackResult(result);
            
            if (guess.Id == _targetMovie.Id)
            {
                //open win panel
                Debug.Log("WIN!!");
            }
            else
            {
                _currentTier++;
                _view.UnlockClue(_currentTier);
            }
        }

        private async UniTask LoadTargetMovieAsync()
        {
            LoadingPanel.Instance.Show();
            
            const int startClueIndex = 0;
            _currentTier = 0;
            _guessedIds.Clear();
            
            var movie = await _moviePickService.GetValidGameMovieAsync();
            if (movie == null)
            {
                return;
            }
            _targetMovie = movie;
            
            var poster = await _apiService.GetMovieImageAsync(movie.PosterPath);
            var backdrop = await _apiService.GetMovieImageAsync(movie.BackdropPath);
            
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
        
        // private async UniTask<DetailsSuperlistModel> GetValidGameMovieAsync()
        // {
        //     LoadingPanel.Instance.Show();   
        //     
        //     const int maxPageSize = 100;
        //     var maxAttempts = 10;
        //     var attempts = 0;
        //
        //     while (attempts < maxAttempts)
        //     {
        //         attempts++;
        //         var randomPageNumber = Random.Range(1, maxPageSize);
        //         var listResponse = await _apiService.GetPopularMoviesAsync(randomPageNumber);
        //
        //         if (listResponse.Results == null || listResponse.Results.Count == 0)
        //         {
        //             continue;
        //         }
        //         
        //         var candidates = ShuffleListRandomly(listResponse.Results);
        //
        //         foreach (var candidate in candidates)
        //         {
        //             if (string.IsNullOrEmpty(candidate.backdrop_path))
        //             {
        //                 continue;
        //             }
        //
        //             if (string.IsNullOrEmpty(candidate.poster_path))
        //             {
        //                 continue;
        //             }
        //
        //             if (candidate.Genre_Ids == null || candidate.Genre_Ids.Count == 0)
        //             {
        //                 continue;
        //             }
        //
        //             var details = await _apiService.GetMovieDetailsAsync(candidate.Id);
        //
        //             if (ValidateMovie(details))
        //             {
        //                 Debug.Log($"Found valid game movie: {details.Title}");
        //                 return details;
        //             }
        //         }
        //     }
        //
        //     LoadingPanel.Instance.Hide();
        //     return null;
        // }

        // private bool ValidateMovie(DetailsSuperlistModel movie)
        // {
        //     if (string.IsNullOrEmpty(movie.Tagline))
        //     {
        //         return false;
        //     }
        //
        //     var hasDirector = false;
        //     foreach (var person in movie.Credits.Crew)
        //     {
        //         if (person.Job == "Director")
        //         {
        //             hasDirector = true;
        //         }
        //     }
        //    
        //     if (!hasDirector)
        //     {
        //         return false;
        //     }
        //
        //     if (movie.Credits.Cast == null || movie.Credits.Cast.Count < 3)
        //     {
        //         return false;
        //     }
        //     
        //     return true;
        // }
        //
        // private List<MovieData> ShuffleListRandomly(List<MovieData> inputList)
        // {    
        //     //take any list of DetailsSuperlistModel and return it with Fischer-Yates shuffle
        //     var i = 0;
        //     var t = inputList.Count;
        //     MovieData p;
        //     var tempList = new List<MovieData>();
        //     tempList.AddRange(inputList);
        //
        //     while (i < t)
        //     {
        //         var r = Random.Range(i, tempList.Count);
        //         p = tempList[i];
        //         tempList[i] = tempList[r];
        //         tempList[r] = p;
        //         i++;
        //     }
        //
        //     return tempList;
        // }

        public void Dispose()
        {
           _searchController.Dispose();
        }
    }
}