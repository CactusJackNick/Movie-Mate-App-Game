using DefaultNamespace;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace SearchSection
{
    public class SearchByNamePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private SearchByNameView _view;
        [SerializeField] private MoviesView  _moviesView;
        
        [Header("Results")]
        [SerializeField] private ScrollRect _scrollRect; 
        
        private SearchByNameController _controller;
        
        public override void Awake()
        {
            base.Awake();
            
            _controller = new SearchByNameController(_moviesView, ApiService.Instance);
            _controller.Initialize(_view);
            
            _view.OnBackButtonPressed += ReturnToSearchScreen;
            _view.OnSearchButtonPressed += OnSubmit;
            
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }
        
        public override void Show()
        {
            base.Show();
            _scrollRect.verticalNormalizedPosition = 1f;
            _controller.LoadInitialMovies();
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
        
        private void OnSubmit(string text)
        {
            _controller.StartSearch(text);
        }

        private void OnDestroy()
        {
            _view.OnSearchButtonPressed -= OnSubmit;
            _view.OnBackButtonPressed -= ReturnToSearchScreen;
        }
    }
}