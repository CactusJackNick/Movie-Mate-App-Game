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
            _moviePickingService = new MoviePickingService(ApiService.Instance, LoadingPanel.Instance);
            _controller = new GameController
            (
                view: _view,
                apiService: ApiService.Instance,
                clueFactory: _clueFactory,
                feedbackService: _feedbackService,
                searchController: _searchGameController,
                moviePickService: _moviePickingService,
                resultsView: _resultsView,
                loadingPanel: LoadingPanel.Instance
            );
            
            _controller.OnBackButtonRequested += ReturnToMain;
            _scrollRect.onValueChanged.AddListener(OnScroll);
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

        private void OnDestroy()
        {
            _controller.OnBackButtonRequested -= ReturnToMain;
            _resultsView.OnExitButtonPressed -= ReturnToMain;
            _scrollRect.onValueChanged.RemoveListener(OnScroll);
            _controller?.Dispose();
        }
    }
}