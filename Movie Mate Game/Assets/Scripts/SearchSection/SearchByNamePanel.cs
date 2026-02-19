using DefaultNamespace;
using DefaultNamespace.Models;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace SearchSection
{
    public class SearchByNamePanel : ABaseUIMediatorComponent
    {
        [Header("Views")]
        [SerializeField] private SearchByNameView _view;
        [SerializeField] private MoviesView _moviesView;
        
        [Header("Results")]
        [SerializeField] private ScrollRect _scrollRect; 
        
        private SearchByNameController _controller;
        private bool _isReturningFromDetails;
        
        public override void Awake()
        {
            base.Awake();
            _controller = new SearchByNameController
            (
                _view,
                _moviesView,
                ApiService.Instance
            );
            _controller.Initialize();
            
            _controller.OnBackRequested += ReturnToSearchScreen;
            _controller.OnSearchButtonRequested += OnSubmit;
            _moviesView.DetailsButtonClicked += GoToDetailsPanel;
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }
        
        public override void Show()
        {
            base.Show();
            if (_isReturningFromDetails)
            {
                _isReturningFromDetails = false;
                return;
            }
            
            _scrollRect.verticalNormalizedPosition = 1f;

            if (!_controller.HasActiveSearch)
            {
                _controller.LoadInitialMovies();
            }
        }
        
        private void OnScroll(Vector2 pos)
        {
            if (pos.y < 0.3f)
            {
                _controller.LoadNextPage();
            }
        }

        private void ReturnToSearchScreen()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.SeachPanel);
            _moviesView.ClearItems();
        }

        private void GoToDetailsPanel(MovieData data)
        { 
            MovieDetailsPanel.TargetMovieId =  data.Id;
            MovieDetailsPanel.PreviousPanel = Panels.SeachByNamePanel;
            
            _isReturningFromDetails =  true;
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.DetailsPanel);
        }
        
        private void OnSubmit(string text)
        {
            _scrollRect.verticalNormalizedPosition = 1f;
            _controller.StartSearch(text);
        }

        private void OnDestroy()
        {
            _controller.OnBackRequested -= ReturnToSearchScreen;
            _controller.OnSearchButtonRequested -= OnSubmit;
            _moviesView.DetailsButtonClicked -= GoToDetailsPanel;
            _controller?.Dispose();
        }
    }
}