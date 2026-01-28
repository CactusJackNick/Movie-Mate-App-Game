using DefaultNamespace.Genre;
using Mediator;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoviePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private MoviesView _view;
        
        private MoviesController _controller;

        public override void Awake()
        {
            base.Awake();
            
            _controller = new MoviesController(_view, ApiService.Instance);
            
            _view.OnBackButtonPressed += ReturnToGenresScreen;
        }

        public override void Show()
        {
            base.Show();
            
            var idsToLoad = GenrePanel.SelectedGenreId;
            
            _controller.LoadFilteredMovies(idsToLoad);
        }
        
        private void ReturnToGenresScreen()
        {
            _controller.filteredList.Clear();
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.Genre);
        }

        private void OnDestroy()
        {
            _view.OnBackButtonPressed -= ReturnToGenresScreen;
        }
    }
}