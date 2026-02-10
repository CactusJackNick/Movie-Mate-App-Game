using Cysharp.Threading.Tasks;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class GamePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private GameView _view;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ResultsView _resultsView; 
        
        private GameController _controller;
        private ClueFactory _clueFactory;
        private FeedbackService _feedbackService;
        private SearchGameController _searchGameController;
        private MoviePickingService _moviePickingService;
        
        public override void Awake()
        {
            base.Awake();

            _clueFactory = new ClueFactory();
            _feedbackService = new FeedbackService();
            _searchGameController = new SearchGameController(_view, ApiService.Instance);
            _moviePickingService = new MoviePickingService(ApiService.Instance);
            _controller = new GameController
            (
                view: _view,
                apiService: ApiService.Instance,
                clueFactory: _clueFactory,
                feedbackService: _feedbackService,
                searchController: _searchGameController,
                moviePickService: _moviePickingService,
                resultsView: _resultsView
            );
            
            _view.OnBackButtonPressed += ReturnToMain;
            _view.OnInputPressed += OnSubmit;
            _view.OnGuessSelected += OnPlayerGuess;
            _view.OnClearTextPressed += ClearSearchBar;
            _scrollRect.onValueChanged.AddListener(OnScroll);

            _resultsView.OnNewGameButtonPressed += OnRestartGame;
            _resultsView.OnExitButtonPressed += ReturnToMain;
        }

        

        public override void Show()
        {
            base.Show();
            
            _scrollRect.verticalNormalizedPosition = 1f;

            _controller.LoadMovies();
        }
        
        private void OnScroll(Vector2 pos)
        {
            if (pos.y < 0.1f)
            {
                _controller.LoadNextPageGuesses();
            }
        }
        
        private void ReturnToMain()
        {
            _resultsView.Hide();
            _view.SetInputStatus(true);
            
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
            
            _view.ClearGuessesItems();
            _view.ClearFeedbackItems();
            
            Resources.UnloadUnusedAssets();
        }
        
        private void OnRestartGame()
        {
            _resultsView.Hide();
            _view.SetInputStatus(true);
            
            _view.ClearGuessesItems();
            _view.ClearFeedbackItems();
            ClearSearchBar();
            
            Resources.UnloadUnusedAssets();
            _controller.LoadMovies();
            _scrollRect.verticalNormalizedPosition = 1f;
        }
        
        private void OnSubmit(string text)
        {
            _controller.StartSearch(text);
        }

        private void ClearSearchBar()
        {
            _searchGameController.ResetSearchState();
        }
        
        private void OnPlayerGuess(int movieId)
        {
            _controller.ProcessPlayerGuess(movieId).Forget();
        }

        private void OnDestroy()
        {
            _view.OnBackButtonPressed -= ReturnToMain;
            _view.OnInputPressed -= OnSubmit;
            _view.OnGuessSelected -= OnPlayerGuess;
            _view.OnClearTextPressed -= ClearSearchBar;
            _scrollRect.onValueChanged.RemoveListener(OnScroll);
            _resultsView.OnNewGameButtonPressed -= OnRestartGame;
            _resultsView.OnExitButtonPressed -= ReturnToMain;
            _controller?.Dispose();
        }
    }
}