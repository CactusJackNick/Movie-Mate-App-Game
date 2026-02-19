using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoviesController : IMovieController
    {
        public List<MovieData> filteredList = new();
        
        private readonly IMovieView _movieView;
        private readonly IApiService _apiService;
        
        private int _currentGenreId;
        private int _currentPage = 1;
        private bool _isLoading = false;
        private CancellationTokenSource _loadCts;
        
        public MoviesController(IMovieView movieView, IApiService apiService)
        {
            _movieView = movieView;
            _apiService = apiService;

            _movieView.OnBackButtonPressed += HandleCloseRequested;
            _movieView.DetailsButtonClicked += HandelDetailsClicked;
        }

        public event Action OnCloseButtonRequested;
        public event Action<MovieData> OnDetailsRequested;

        public async UniTask LoadFilteredMovies(int targetId)
        {
            CancelPendingRequests();
            _loadCts = new CancellationTokenSource();
            
            _currentGenreId = targetId;
            _currentPage = 1; 
            _isLoading = false;
            
            _movieView.ClearItems();
            
            await FetchMoviesAsync(isChecking: false, _loadCts.Token);
        }
        
        public async UniTask LoadNextPage()
        {
            if (_isLoading || _loadCts == null)
            {
                return;
            }

            _currentPage++;
            await FetchMoviesAsync(isChecking: true, _loadCts.Token);
        }
        
        public void CancelPendingRequests()
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = null;
            
            _isLoading = false;
            _movieView.SetLoadingSpinnerState(false);
        }

        private async UniTask FetchMoviesAsync(bool isChecking, CancellationToken token)
        {
            _movieView.SetLoadingSpinnerState(true);
            _isLoading = true;

            try
            {
                var response = await _apiService.GetMoviesByGenreAsync(_currentGenreId, _currentPage);
                if (token.IsCancellationRequested)
                {
                    return;
                }

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
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    Debug.LogError(ex.Message);
                }
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    _isLoading = false;
                    _movieView.SetLoadingSpinnerState(false);
                }
            }
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
            CancelPendingRequests();
            _movieView.DetailsButtonClicked -= HandelDetailsClicked;
            _movieView.OnBackButtonPressed -= HandleCloseRequested;
        }
    }
}