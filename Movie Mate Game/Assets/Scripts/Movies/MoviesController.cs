using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public class MoviesController : IMovieController
    {
        public List<MovieData> filteredList = new();
        
        private readonly IMovieView _movieView;
        private readonly IApiService  _apiService;
        
        private int _currentGenreId;
        private int _currentPage = 1;
        private bool _isLoading = false;
        
        public MoviesController(IMovieView movieView, IApiService apiService)
        {
            _movieView = movieView;
            _apiService = apiService;
        }

        public void LoadFilteredMovies(int targetId)
        {
            _currentGenreId = targetId;
            _currentPage = 1; 
            _isLoading = false;
            
            _movieView.ClearItems();
            
            FetchMoviesAsync(isChecking: false).Forget();
        }
        
        public void LoadNextPage()
        {
            if (_isLoading)
            {
                return;
            }

            _currentPage++;
            FetchMoviesAsync(true).Forget();
        }
        
        private async UniTask FetchMoviesAsync(bool isChecking)
        {
            _isLoading = true;
            LoadingPanel.Instance.Show();
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
            else
            {
                if (!isChecking)
                {
                    _movieView.ShowNoMoviesText(true);
                }
            }
            
            _isLoading = false;
            LoadingPanel.Instance.Hide();
        }
    }
}