using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public class MoviesController : IMovieController
    {
        public List<MovieData> filteredList = new();
        
        private readonly IMovieView _movieView;
        private readonly IApiService _apiService;
        private readonly ILoadingPanel _loadingPanel;
        
        private int _currentGenreId;
        private int _currentPage = 1;
        private bool _isLoading = false;
        
        public MoviesController(IMovieView movieView, IApiService apiService, ILoadingPanel loadingPanel)
        {
            _movieView = movieView;
            _apiService = apiService;
            _loadingPanel = loadingPanel;

            _movieView.OnBackButtonPressed += HandleCloseRequested;
            _movieView.DetailsButtonClicked += HandelDetailsClicked;
        }

        public event Action OnCloseButtonRequested;
        public event Action<MovieData> OnDetailsRequested;

        public async UniTask LoadFilteredMovies(int targetId)
        {
            _currentGenreId = targetId;
            _currentPage = 1; 
            _isLoading = false;
            
            _movieView.ClearItems();
            
            await FetchMoviesAsync(isChecking: false);
        }
        
        public async UniTask LoadNextPage()
        {
            if (_isLoading)
            {
                return;
            }

            _currentPage++;
            await FetchMoviesAsync(isChecking: true);
        }
        
        private async UniTask FetchMoviesAsync(bool isChecking)
        {
            _loadingPanel.Show();
            _isLoading = true;
            var response = await _apiService.GetMoviesByGenreAsync(_currentGenreId, _currentPage);

            if (response is { Results: { Count: > 0 } })
            {
                if (isChecking)
                {
                    _movieView.AddMovies(response.Results);
                }
                else
                {
                    _movieView.DisplayMovies(response.Results);
                }
            }
            
            _isLoading = false;
            _loadingPanel.Hide();
        }
        
        private void HandelDetailsClicked(MovieData data)
        {
            OnDetailsRequested?.Invoke(data);
        }
        
        private void HandleCloseRequested()
        {
            OnCloseButtonRequested?.Invoke();
        }
        
        public void Dispose()
        {
            _movieView.DetailsButtonClicked -= HandelDetailsClicked;
            _movieView.OnBackButtonPressed -= HandleCloseRequested;
        }
    }
}