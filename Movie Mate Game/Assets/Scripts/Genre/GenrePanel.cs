using Mediator;
using UnityEngine;

namespace DefaultNamespace.Genre
{
    public class GenrePanel : ABaseUIMediatorComponent
    {
        public static int SelectedGenreId;
        
        [SerializeField] private GenreView _view;
        [SerializeField] private GenreIconsConfig _config;
        
        private GenreController _controller;
        
        public override void Awake()
        {
            base.Awake();
            
            _view.OnBackClicked += ReturnToMain;

            _controller = new GenreController(_view, ApiService.Instance, _config);

            _view.OnGenreClicked += HandleGenreClicked;
        }
        public override void Show()
        {
            base.Show();
            _controller.LoadGenres();
        }
        
        private void ReturnToMain()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
        }

        private void HandleGenreClicked(int genreId)
        {
            SelectedGenreId = genreId;
            UINavigationMediator.Instance.ReplacePanel(Panels.Genre, Panels.Movies);
        }
        
        private void OnDestroy()
        {
            _view.OnBackClicked -= ReturnToMain;
            _view.OnGenreClicked -= HandleGenreClicked;
            //_controller?.Dispose();
        }
    }
}