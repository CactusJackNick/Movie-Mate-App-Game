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
        private readonly IResultsView _resultsView;
        private readonly HashSet<int> _guessedIds = new();

        private DetailsSuperlistModel _targetMovie;
        private int _currentTier = 0;

        public GameController(IGameView view, IApiService apiService
        , IClueFactory clueFactory, IFeedbackService feedbackService,
        ISearchController searchController, IMoviePickService moviePickService, IResultsView resultsView)
        {
            _view = view;
            _apiService = apiService;
            _clueFactory = clueFactory;
            _feedbackService = feedbackService;
            _searchController = searchController;
            _moviePickService = moviePickService;
            _resultsView = resultsView;
            
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
                _view.SetInputStatus(false);
                _resultsView.ShowResultsAsync(true, null).Forget();
            }
            else if (_currentTier >= 5)
            {
                _view.SetInputStatus(false);
                await UniTask.Delay(1500);
                _resultsView.ShowResultsAsync(false, _targetMovie.Title).Forget();
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

        public void Dispose()
        {
           _searchController.Dispose();
        }
    }
}