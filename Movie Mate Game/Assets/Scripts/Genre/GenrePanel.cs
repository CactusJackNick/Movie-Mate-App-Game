using Cysharp.Threading.Tasks;
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
            
            _controller = new GenreController
            (
                view: _view,
                apiService: ApiService.Instance,
                config: _config,
                localizationService: LocalizationManager.Instance
            );
            
            _view.OnBackClicked += ReturnToGeneralSearch;
            _controller.OnGenreSelected += OnGenreSelected;
        }
        public override void Show()
        {
            base.Show();
            _controller.LoadGenresAsync().Forget();
        }
        
        private void ReturnToGeneralSearch()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.SeachPanel);
        }

        private void OnGenreSelected(int genreId)
        {
            SelectedGenreId = genreId;
            UINavigationMediator.Instance.ReplacePanel(Panels.Genre, Panels.Movies);
        }
        
        private void OnDestroy()
        {
            if (_controller != null)
            {
                _controller.OnGenreSelected -= OnGenreSelected;
                _controller.Dispose();
            }
            
            _view.OnBackClicked -= ReturnToGeneralSearch;
        }
    }
}