using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Models;
using NSubstitute;
using NUnit.Framework;

namespace Tests
{
    public class MoviesControllerTests
    {
        private IMovieView _view;
        private IApiService _apiService;
        private ILoadingPanel _loadingPanel;
        
        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<IMovieView>();
            _apiService = Substitute.For<IApiService>();
            _loadingPanel = Substitute.For<ILoadingPanel>();
        }

        [TearDown]
        public void TearDown()
        {
            _view = null;
            _apiService = null;
            _loadingPanel = null;
        }

        [Test]
        public async Task LoadFilteredMovies_WithValidId_ClearsView()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService, _loadingPanel);
            
            // Act
            await sut.LoadFilteredMovies(1);

            // Assert
            _view.Received(1).ClearItems();
        }
        
        [Test]
        public async Task LoadFilteredMovies_ApiReturnsData_DisplaysMovies()
        {
            // Arrange
            const int targetId = 10;
            var sut = new MoviesController(_view, _apiService, _loadingPanel);
            
            var mockData = new List<MovieData>
            {
                new()
                {
                    Id = 1
                }
            };
            
            var response = new MovieListResponse
            {
                Results = mockData
            };

            _apiService.GetMoviesByGenreAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(UniTask.FromResult(response));

            // Act
            await sut.LoadFilteredMovies(targetId);

            // Assert
            _view.Received(1).DisplayMovies(mockData);
        }
        
        [Test]
        public async Task LoadNextPage_ApiReturnsData_AddsMoviesToExistingList()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService, _loadingPanel);
            var mockData = new List<MovieData>
            { 
                new()
                {
                    Id = 2
                }
                
            };
            
            _apiService.GetMoviesByGenreAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(UniTask.FromResult(
                    new MovieListResponse
                    {
                        Results = mockData
                    }));

            await sut.LoadFilteredMovies(Arg.Any<int>()); 

            // Act
            await sut.LoadNextPage();

            // Assert
            _view.Received(1).AddMovies(mockData);
        }
        
        [Test]
        public async Task LoadFilteredMovies_DuringCall_ShowsAndHidesLoading()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService, _loadingPanel);
            
            _apiService.GetMoviesByGenreAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(UniTask.FromResult(new MovieListResponse()));

            // Act
            await sut.LoadFilteredMovies(1);

            // Assert
            Received.InOrder(() =>
            {
                _loadingPanel.Show();
                _apiService.GetMoviesByGenreAsync(Arg.Any<int>(), Arg.Any<int>());
                _loadingPanel.Hide();
            });
        }
    }
}