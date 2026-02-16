using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Game;
using DefaultNamespace.Models;
using NSubstitute;
using NUnit.Framework;
    
public class SearchGameControllerTests
{
    private IGameView _view;
    private IApiService _api;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IGameView>();
        _api = Substitute.For<IApiService>();
    }

    [TearDown]
    public void TearDown()
    {
        _view = null;
        _api = null;
    }

    [Test]
    public async Task StartSearch_ValidQuery_WaitsDebounceAndCallsApi()
    {
        // Arrange
        var sut = new SearchGameController(_view, _api);
        var query = "Batman";
        var response = new MovieListResponse
        {
            Results = new List<MovieData>(),
            TotalPages = 1
        };
        _api.SearchMoviesAsync(query, 1).Returns(UniTask.FromResult(response));

        // Act
        sut.StartSearch(query);
        await UniTask.Delay(750); 

        // Assert
        _ = _api.Received(1).SearchMoviesAsync(query, 1);
        _view.Received(1).DisplayMovies(Arg.Any<List<MovieData>>());
    }

    [Test]
    public async Task StartSearch_EmptyQuery_ClearsViewAndDoesNotCallApi()
    {
        // Arrange
        var sut = new SearchGameController(_view, _api);

        // Act
        sut.StartSearch("");
        await UniTask.Delay(750); 

        // Assert
        _view.Received(2).ClearGuessesItems();
        _ = _api.DidNotReceive().SearchMoviesAsync(Arg.Any<string>(), Arg.Any<int>());
    }

    [Test]
    public async Task StartSearch_RapidTyping_CancelsPreviousRequest()
    {
        // Arrange
        var sut = new SearchGameController(_view, _api);
        _api.SearchMoviesAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(UniTask.FromResult(new MovieListResponse
            {
                Results = new List<MovieData>()
            }));

        // Act
        sut.StartSearch("Bat");
        await UniTask.Delay(100);
        
        sut.StartSearch("Batman");
        await UniTask.Delay(750);

        // Assert
        _ = _api.DidNotReceive().SearchMoviesAsync("Bat", 1);
        _ = _api.Received(1).SearchMoviesAsync("Batman", 1);
    }

    [Test]
    public async Task SearchMovies_FiltersOutInvalidAndGuessedMovies()
    {
        // Arrange
        var sut = new SearchGameController(_view, _api);
        var guessedId = 99;
        
        var filterSet = new HashSet<int>
        {
            guessedId
        };
        sut.SetFilter(filterSet);
        
        var apiResults = new List<MovieData>
        {
            new() {
                Id = 1,
                Title = "Valid",
                Poster_path = "path",
                Backdrop_Path = "path"
            },
            new()
            {
                Id = guessedId,
                Title = "Already Guessed",
                Poster_path = "path", 
                Backdrop_Path = "path"
            }
        };
        var response = new MovieListResponse
        {
            Results = apiResults,
            TotalPages = 1
        };
        _api.SearchMoviesAsync("Test", 1).Returns(UniTask.FromResult(response));

        // Act
        sut.StartSearch("Test");
        await UniTask.Delay(750);

        // Assert
        _view.Received(1).DisplayMovies(Arg.Is<List<MovieData>>(list => list.Count == 1));
    }

    [Test]
    public async Task LoadNextPage_ValidState_IncrementsPageAndAddsMovies()
    {
        // Arrange
        var sut = new SearchGameController(_view, _api);
        var query = "Test";
        
        var page1 = new MovieListResponse
        {
            TotalPages = 5,
            Results = new List<MovieData>()
        };
        var page2 = new MovieListResponse
        {
            TotalPages = 5,
            Results = new List<MovieData>()
        };
        
        _api.SearchMoviesAsync(query, 1).Returns(UniTask.FromResult(page1));
        _api.SearchMoviesAsync(query, 2).Returns(UniTask.FromResult(page2));

        // Act
        sut.StartSearch(query);
        await UniTask.Delay(750); 
        
        sut.LoadNextPage();
        await UniTask.Delay(50);

        // Assert
        _ = _api.Received(1).SearchMoviesAsync(query, 2); 
        _view.Received(1).AddMovies(Arg.Any<List<MovieData>>()); 
    }

    [Test]
    public async Task LoadNextPage_MaxPagesReached_DoesNothing()
    {
        // Arrange
        var sut = new SearchGameController(_view, _api);
        var query = "Test";
        
        var page1 = new MovieListResponse
        {
            TotalPages = 1, 
            Results = new List<MovieData>()
        };
        _api.SearchMoviesAsync(query, 1).Returns(UniTask.FromResult(page1));

        // Act
        sut.StartSearch(query);
        await UniTask.Delay(750); 
        
        sut.LoadNextPage();
        await UniTask.Delay(50);

        // Assert
        _ = _api.DidNotReceive().SearchMoviesAsync(query, 2);
    }
}