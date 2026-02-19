using System;
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
        
        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<IMovieView>();
            _apiService = Substitute.For<IApiService>();
        }

        [TearDown]
        public void TearDown()
        {
            _view = null;
            _apiService = null;
        }

        [Test]
        public async Task LoadFilteredMovies_WithValidId_ClearsView()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService);
            
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
            var sut = new MoviesController(_view, _apiService);
            
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
            var sut = new MoviesController(_view, _apiService);
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

            await sut.LoadFilteredMovies(1); 

            // Act
            await sut.LoadNextPage();

            // Assert
            _view.Received(1).AddMovies(mockData);
        }
        
        [Test]
        public async Task LoadFilteredMovies_DuringCall_ShowsAndHidesLoadingPanel()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService);
            
            _apiService.GetMoviesByGenreAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(UniTask.FromResult(new MovieListResponse()));

            // Act
            await sut.LoadFilteredMovies(1);

            // Assert
            Received.InOrder(() =>
            { 
                _view.SetLoadingSpinnerState(false);
                _view.SetLoadingSpinnerState(true);
                _apiService.GetMoviesByGenreAsync(Arg.Any<int>(), Arg.Any<int>());
                _view.SetLoadingSpinnerState(false);
            });
        }

        [Test]
        public void OnDetailsRequested_Raises_View_OnDetailsButtonClicked()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService);
            MovieData capturedMovie = null;
            
            var movieObj = new MovieData()
            {
                Id = 100
            };
            
            // Act
            sut.OnDetailsRequested += HandleDetailsRequested;
            _view.DetailsButtonClicked += Raise.Event<Action<MovieData>>(movieObj);
            
            //Assert
            Assert.IsNotNull(capturedMovie);
            Assert.AreEqual(movieObj, capturedMovie);
            return;
            
            void HandleDetailsRequested(MovieData obj)
            {
                capturedMovie = obj;
            }
        }

        [Test]
        public void OnCloseRequested_View_RaisesOnBackButtonPressed()
        {
            // Arrange
            var sut = new MoviesController(_view, _apiService);
            var closeRequestedCalled =  false;

            // Act
            sut.OnCloseButtonRequested += HandleCloseRequested;
            _view.OnBackButtonPressed += Raise.Event<Action>();
            
            //Assert
            Assert.IsTrue(closeRequestedCalled);
            return;
        
            void HandleCloseRequested()
            {
                closeRequestedCalled =  true;
            }
        }
    }
}