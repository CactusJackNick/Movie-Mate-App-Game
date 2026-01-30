using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Models;

namespace SearchSection
{
    public class SearchByNameController : ISearchByNameController
    {
        private readonly IMovieView _movieView;
        private readonly IApiService _apiService;

        private ISearchByNameView _view;
        private string _currentQuery = "";
        private int _currentPage = 1;
        private bool _isLoading;

        public SearchByNameController(IMovieView movieView, IApiService apiService)
        {
            _movieView = movieView;
            _apiService = apiService;
        }
        
        public bool HasActiveSearch => !string.IsNullOrEmpty(_currentQuery);

        public void Initialize(ISearchByNameView view)
        {
            _view = view;
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

            PopulateMovies(_currentQuery, _currentPage).Forget();
        }

        public void LoadNextPage()
        {
            if (_isLoading)
            {
                return;
            }

            _currentPage++;
            PopulateMovies(_currentQuery, _currentPage).Forget();
        }

        private async UniTask PopulateMovies(string query, int page)
        {
            LoadingPanel.Instance.Show();
            _isLoading = true;

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
                if (page == 1)
                {
                    _movieView.DisplayMovies(response.Results);
                }
                else
                {
                    _movieView.AddMovies(response.Results);
                }
            }

            _isLoading = false;
            LoadingPanel.Instance.Hide();
        }
    }
}