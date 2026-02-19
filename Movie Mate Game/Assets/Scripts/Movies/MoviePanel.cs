using Cysharp.Threading.Tasks;
using DefaultNamespace.Genre;
using DefaultNamespace.Models;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MoviePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private MoviesView _view;
        [SerializeField] private ScrollRect _scrollRect;
        
        private MoviesController _controller;
        private bool _isReturningFromDetails;
        private bool _isNavigatingAway;

        public override void Awake()
        {
            base.Awake();
            
            _controller = new MoviesController(_view, ApiService.Instance);
            
            _controller.OnCloseButtonRequested += ReturnToGenresScreen;
            _controller.OnDetailsRequested += OpenDetails;
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        public override void Show()
        {
            base.Show();
            _isNavigatingAway = false;
            
            if (_isReturningFromDetails)
            {
                _isReturningFromDetails = false;
                return;
            }
            _scrollRect.verticalNormalizedPosition = 1f;
            
            var idsToLoad = GenrePanel.SelectedGenreId;
            
            _controller.LoadFilteredMovies(idsToLoad).Forget();
        }
        
        private void OnScroll(Vector2 pos)
        {
            if (_isNavigatingAway)
            {
                return;
            }
            
            if (pos.y < 0.3)
            {
                _controller.LoadNextPage().Forget();
            }
        }
        
        private void ReturnToGenresScreen()
        {
            _isNavigatingAway = true;
            _controller.CancelPendingRequests();
            _controller.filteredList.Clear();
            _view.ClearItems();
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.Genre);
        }
        
        private void OpenDetails(MovieData data)
        {
            _isNavigatingAway = true;
            _controller.CancelPendingRequests();
            
            MovieDetailsPanel.TargetMovieId = data.Id;
            MovieDetailsPanel.PreviousPanel = Panels.Movies;
            
            _isReturningFromDetails =  true;
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.DetailsPanel);
        }

        private void OnDestroy()
        {
            if (_controller is null)
            {
                return;
            }
            _controller.OnCloseButtonRequested -= ReturnToGenresScreen;
            _controller.OnDetailsRequested -= OpenDetails;
            _scrollRect.onValueChanged.RemoveListener(OnScroll);
            _controller.Dispose();
        }
    }
}