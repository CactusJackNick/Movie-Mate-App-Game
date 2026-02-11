using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Models;
using UnityEngine;

namespace SearchSection
{
    public class SearchByNameController : ISearchByNameController
    {
        private readonly IMovieView _movieView;
        private readonly IApiService _apiService;
        private readonly ILoadingPanel _loadingPanel;
        private readonly ISearchByNameView _view;
        
        private string _currentQuery = "";
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _isLoading;
        private CancellationTokenSource _cts;

        public SearchByNameController(ISearchByNameView view, IMovieView movieView,
            IApiService apiService, ILoadingPanel loadingPanel)
        {
            _view = view;
            _movieView = movieView;
            _apiService = apiService;
            _loadingPanel = loadingPanel;

            _view.OnBackButtonPressed += HandleBackRequest;
            _view.OnSearchButtonPressed += HandleSearchRequest;
        }

        public event Action OnBackRequested;
        public event Action<string> OnSearchButtonRequested;
        
        public bool HasActiveSearch => !string.IsNullOrEmpty(_currentQuery);

        public void Initialize()
        {
            _view.SetupViewInitialState();
        }

        public void LoadInitialMovies()
        {
            StartSearch("");
        }

        public void StartSearch(string search)
        {
            _currentQuery = search;
            _currentPage = 1;
            _isLoading = false;

            if (_cts is not null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            
            _cts =  new CancellationTokenSource();
            SearchWithDebounce(search, _cts.Token).Forget();
        }

        public void LoadNextPage()
        {
            if (_isLoading || _currentPage >= _totalPages)
            {
                return;
            }

            _currentPage++;
            PopulateMovies(_currentQuery, _currentPage).Forget();
        }

        private async UniTaskVoid SearchWithDebounce(string query, CancellationToken token)
        {
            const int debounceTime = 500;
            await UniTask.Delay(debounceTime, cancellationToken: token);
            var page = 1;
            await PopulateMovies(query, page);
        }

        private async UniTask PopulateMovies(string query, int page)
        {
            if (page > _totalPages)
            {
                return;
            }
            
            _isLoading = true;
            _loadingPanel.Show();

            try
            {
                MovieListResponse response;

                if (!string.IsNullOrEmpty(query))
                {
                    response = await _apiService.SearchMoviesAsync(query, page);
                }
                else
                {
                    response = await _apiService.GetPopularMoviesAsync(page);
                }

                if (response is { Results: not null })
                {
                    _totalPages =  response.TotalPages;
                    
                    if (page == 1)
                    {
                        _movieView.DisplayMovies(response.Results);
                    }
                    else
                    {
                        _movieView.AddMovies(response.Results);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                _isLoading = false;
                _loadingPanel.Hide();
            }
        }
        
        private void HandleBackRequest()
        {
            OnBackRequested?.Invoke();
            _movieView.ClearItems();
        }
        
        private void HandleSearchRequest(string obj)
        {
            OnSearchButtonRequested?.Invoke(obj);
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _view.OnBackButtonPressed -= HandleBackRequest;
            _view.OnSearchButtonPressed -= HandleSearchRequest;
        }
    }
}