using DefaultNamespace;
using DefaultNamespace.Game;
using NSubstitute;
using NUnit.Framework;

public class GameControllerTests
{
    private IGameView _view;
    private IApiService _apiService;
    private ISearchController _searchController;
    private IFeedbackService _feedbackService;
    private IClueFactory _clueFactory;
    private IMoviePickService _movieRepository;
    private IResultsView _resultsView;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IGameView>(); 
        _apiService = Substitute.For<IApiService>();
        _searchController = Substitute.For<ISearchController>();
        _feedbackService = Substitute.For<IFeedbackService>();
        _clueFactory = Substitute.For<IClueFactory>();
        _movieRepository = Substitute.For<IMoviePickService>();
        _resultsView = Substitute.For<IResultsView>();
    }

    [TearDown]
    public void TearDown()
    {
        _view = null;
        _apiService = null;
        _searchController = null;
        _feedbackService = null;
        _clueFactory = null;
        _movieRepository = null;
        _resultsView = null;
    }

    [Test]
    public void LoadMovies_WhenFound_DisplaysClues()
    {
        // Arrange
        var controller = new GameController(
            _view, 
            _apiService,
            _clueFactory,
            _feedbackService, 
            _searchController, 
            _movieRepository,
            _resultsView
        );

        // Act
        controller.LoadMovies();

        // Assert
        //_view.Received(1).DisplayClues(dummyClues);
        _view.Received(1).UnlockClue(0);
    }
}