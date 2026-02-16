using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Game;
using DefaultNamespace.Models;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

public class GameControllerTests
{
    private IGameView _view;
    private IApiService _api;
    private ISearchController _searchController;
    private IFeedbackService _feedbackService;
    private IClueFactory _clueFactory;
    private IMoviePickService _moviePickService;
    private IResultsView _resultsView;
    private ILoadingPanel _loadingPanel;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IGameView>(); 
        _api = Substitute.For<IApiService>();
        _searchController = Substitute.For<ISearchController>();
        _feedbackService = Substitute.For<IFeedbackService>();
        _clueFactory = Substitute.For<IClueFactory>();
        _moviePickService = Substitute.For<IMoviePickService>();
        _resultsView = Substitute.For<IResultsView>();
        _loadingPanel = Substitute.For<ILoadingPanel>();
    }

    [TearDown]
    public void TearDown()
    {
        _view = null;
        _api = null;
        _searchController = null;
        _feedbackService = null;
        _clueFactory = null;
        _moviePickService = null;
        _resultsView = null;
        _loadingPanel = null;
    }

    [Test]
    public async Task LoadMovies_SuccessfullyStartsGame()
    {
        // Arrange
        var sut = CreateSut();

        var dummyMovie = new DetailsSuperlistModel 
        { 
            Id = 1, 
            Title = "Target Movie",
            PosterPath = "poster", 
            BackdropPath = "back" 
        };
        
        _moviePickService.GetValidGameMovieAsync()
            .Returns(UniTask.FromResult(dummyMovie));
        
        _api.GetMovieImageAsync(dummyMovie.PosterPath)
            .Returns(UniTask.FromResult(Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero)));
        
        var dummyClues = new List<ClueData>()
        {
            new()
        };
        _clueFactory.AssignDataToClues(dummyMovie, Arg.Any<Sprite>(), Arg.Any<Sprite>())
            .ReturnsForAnyArgs(dummyClues);

        // Act
        sut.LoadMovies();
        
        await UniTask.Delay(100); 

        // Assert
        _searchController.Received(1).ResetSearchState();
        _view.Received(1).DisplayClues(dummyClues);
        _view.Received(1).UnlockClue(0); 
    }
    
    [Test]
    public async Task ProcessPlayerGuess_CorrectGuess_TriggersWin()
    {
        // Arrange
        var sut = CreateSut();
        var targetId = 100;
        ConfigureMocksForGame(targetId);
        
        sut.LoadMovies();
        await UniTask.Delay(50);

        var guessDetails = new DetailsSuperlistModel
        {
            Id = targetId, 
            Title = "Correct Movie"
        };
        _api.GetMovieDetailsAsync(targetId).Returns(UniTask.FromResult(guessDetails));
        
        // Act
        await sut.ProcessPlayerGuess(targetId);

        // Assert
        _view.Received(1).ShowFeedbackResult(Arg.Any<GuessResultModel>());
        _view.Received(1).SetInputStatus(false);
        _ = _resultsView.Received(1).ShowResultsAsync(true, null);
    }

    [Test]
    public async Task ProcessPlayerGuess_WrongGuess_UnlocksNextClue()
    {
        // Arrange
        var sut = CreateSut();
        var targetId = 100;
        var wrongId = 50;
        ConfigureMocksForGame(targetId);
        
        sut.LoadMovies();
        await UniTask.Delay(50);

        var guessDetails = new DetailsSuperlistModel
        {
            Id = wrongId,
            Title = "Wrong Movie"
        };
        _api.GetMovieDetailsAsync(wrongId).Returns(UniTask.FromResult(guessDetails));

        // Act
        await sut.ProcessPlayerGuess(wrongId);

        // Assert
        _view.Received(1).ShowFeedbackResult(Arg.Any<GuessResultModel>());
        _view.DidNotReceive().SetInputStatus(false); 
        _view.Received(1).UnlockClue(1);
    }

    [Test]
    public async Task ProcessPlayerGuess_MaxGuessesReached_TriggersLoss()
    {
        // Arrange
        var sut = CreateSut();
        var targetId = 100;
        var wrongId = 50;
        ConfigureMocksForGame(targetId);
        
        sut.LoadMovies();
        await UniTask.Delay(50);

        var guessDetails = new DetailsSuperlistModel
        {
            Id = wrongId,
            Title = "Wrong Movie"
        };
        _api.GetMovieDetailsAsync(wrongId).Returns(UniTask.FromResult(guessDetails));

        // Act
        for (var i = 0; i <= 5; i++)
        {
            await sut.ProcessPlayerGuess(wrongId);
        }

        // Assert
        _view.Received(1).SetInputStatus(false);
        _ = _resultsView.Received(1).ShowResultsAsync(false, "Target Movie"); 
    }
    
    [Test]
    public void StartSearch_DelegatesToSearchController()
    {
        // Arrange
        var sut = CreateSut();        
        
        // Act
        sut.StartSearch("test");

        // Assert
        _searchController.Received(1).StartSearch("test");
    }
    
    [Test]
    public void OnBackButtonRequested_View_RaisesOnBackButtonPressed()
    {
        // Arrange
        var sut = CreateSut();
        var closeRequestedCalled =  false;

        // Act
        sut.OnBackButtonRequested += HandleCloseRequested;
        _view.OnBackButtonPressed += Raise.Event<Action>();
            
        //Assert
        Assert.IsTrue(closeRequestedCalled);
        return;
        
        void HandleCloseRequested()
        {
            closeRequestedCalled =  true;
        }
    }
    
    [Test]
    public void OnInputPressed_View_TriggersStartSearch()
    {
        // Arrange
        const string input = "Batman";
        CreateSut();
         
        // Act
        _view.OnInputPressed += Raise.Event<Action<string>>(input);
        
        // Assert
        _searchController.Received(1).StartSearch(input);
    }

    [Test]
    public void OnClearTextPressed_View_ResetsSearchState()
    { 
        // Arrange
        CreateSut();
        
        //Act
        _view.OnClearTextPressed += Raise.Event<Action>();
        
        // Assert
        _searchController.Received(1).ResetSearchState();
    }

    [Test]
    public void OnNewGameButtonPressed_ResultsView_RestartsGame()
    { 
        // Arrange
        CreateSut();
    
        // Act
        _resultsView.OnNewGameButtonPressed += Raise.Event<Action>();

        // Assert
        _resultsView.Received(1).Hide();
        _view.Received(1).ClearGuessesItems();
        _searchController.Received(1).ResetSearchState();
    }
    
    private GameController CreateSut()
    {
        return new GameController(
            _view,
            _api,
            _clueFactory,
            _feedbackService,
            _searchController,
            _moviePickService,
            _resultsView,
            _loadingPanel
        );
    }

    private void ConfigureMocksForGame(int id)
    {
        var movie = new DetailsSuperlistModel
        {
            Id = id,
            Title = "Target Movie",
            PosterPath = "p",
            BackdropPath = "b"
        };
        
        _moviePickService.GetValidGameMovieAsync()
            .Returns(UniTask.FromResult(movie));
        _api.GetMovieImageAsync(Arg.Any<string>())
            .Returns(UniTask.FromResult(Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero)));
        _clueFactory.AssignDataToClues(Arg.Any<DetailsSuperlistModel>(), Arg.Any<Sprite>(), Arg.Any<Sprite>())
            .Returns(new List<ClueData>());
        _feedbackService.EvaluateGuess(Arg.Any<DetailsSuperlistModel>(), Arg.Any<DetailsSuperlistModel>())
            .Returns(new GuessResultModel());
    }
}