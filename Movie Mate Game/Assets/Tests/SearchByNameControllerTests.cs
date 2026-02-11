using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Models;
using NSubstitute;
using NUnit.Framework;
using SearchSection;

namespace Tests
{
    public class SearchByNameControllerTests
    {
        private ISearchByNameView _view;
        private IMovieView _mockMovieView;
        private IApiService _api;
        private ILoadingPanel _loadingPanel;

        private const int DebounceTime = 600;

        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<ISearchByNameView>();
            _mockMovieView = Substitute.For<IMovieView>();
            _api = Substitute.For<IApiService>();
            _loadingPanel = Substitute.For<ILoadingPanel>();
        }

        [TearDown]
        public void TearDown()
        {
            _view = null;
            _mockMovieView = null;
            _loadingPanel = null;
            _api = null;
        }

        [Test]
        public async Task StartSearch_WithQuery_ApiService_Calls_SearchMoviesAsync()
        {
            // Arrange
            var sut = new SearchByNameController(_view, _mockMovieView, _api, _loadingPanel);
            const string query = "Inception";
            sut.Initialize();
            
            // Act
            sut.StartSearch(query);
            await UniTask.Delay(DebounceTime);
            
            // Assert
            _ = _api.Received(1).SearchMoviesAsync("Inception", 1);
        }

        [Test]
        public async Task StartSearch_WithoutQuery_ApiService_Calls_GetPopularMoviesAsync()
        {
            // Arrange
            var sut = new SearchByNameController(_view, _mockMovieView, _api, _loadingPanel);
            const string query = "";
            sut.Initialize();
            
            // Act
            sut.StartSearch(query);
            await UniTask.Delay(DebounceTime);
            
            // Assert
            _ = _api.Received(1).GetPopularMoviesAsync(1);
        }

        [Test]
        public async Task LoadNextPage_IncrementsPageCounter()
        {
            // Arrange
            var sut = new SearchByNameController(_view, _mockMovieView, _api, _loadingPanel);
            const int pageCounter = 2;
            const string query = "";
            sut.Initialize();
            
            var response = new MovieListResponse 
            { 
                TotalPages = 2, 
                Results = new List<MovieData>() 
            };
            _api.GetPopularMoviesAsync(1).Returns(UniTask.FromResult(response));
            
            // Act
            sut.StartSearch(query);
            await UniTask.Delay(DebounceTime);
            sut.LoadNextPage();
            await UniTask.Delay(DebounceTime);
            
            // Assert
            Assert.AreEqual(pageCounter, 2);
            _ = _api.Received(1).GetPopularMoviesAsync(pageCounter);
        }

        [Test]
        public async Task OnSearchButtonRequested_Controller_WithDefinedQuery_StartsNewSearch()
        {
            // Arrange
            var sut = new SearchByNameController(_view, _mockMovieView, _api, _loadingPanel);
            const string search = "Inception";
            string receivedQuery = null;
            sut.Initialize();
            
            // Act
            sut.OnSearchButtonRequested += HandleSearchRequest;
            _view.OnSearchButtonPressed += Raise.Event<Action<string>>(search);
            
            sut.StartSearch(search);
            await UniTask.Delay(DebounceTime);
            
            // Assert
            Assert.IsNotNull(receivedQuery);
            Assert.AreEqual(search, receivedQuery);
            _ = _api.Received(1).SearchMoviesAsync(search, 1);
            return;

            void HandleSearchRequest(string query)
            {
                receivedQuery = query;
            }
        }

        [Test]
        public void OnBackRequested_Controller_Raises_OnBackButtonPressed()
        {
            // Arrange
            var sut = new SearchByNameController(_view, _mockMovieView, _api, _loadingPanel);
            var closeRequestedCalled =  false;
            sut.Initialize();
            
            // Act
            sut.OnBackRequested += HandleBackRequest;
            _view.OnBackButtonPressed += Raise.Event<Action>();
            
            // Assert
            Assert.IsTrue(closeRequestedCalled);
            _mockMovieView.Received(1).ClearItems();
            return;
            
            void HandleBackRequest()
            {
                closeRequestedCalled = true;
            }
        }
        
        [Test]
        public async Task LoadNextPage_WhenOnLastPage_DoesNotCallApi()
        {
            // Arrange
            var sut = new SearchByNameController(_view, _mockMovieView, _api, _loadingPanel);
            var response = new MovieListResponse()
            {
                TotalPages = 1,
                Results = new List<MovieData>()
            };
            _ = _api.SearchMoviesAsync(Arg.Any<string>(), 1)
                .Returns(UniTask.FromResult(response));

            sut.StartSearch("Jeff Bezos");
            await Task.Delay(600);

            // Act
            sut.LoadNextPage();
            await Task.Yield();

            // Assert
            _ = _api.DidNotReceive().SearchMoviesAsync(Arg.Any<string>(), 2);
        }
    }
}
