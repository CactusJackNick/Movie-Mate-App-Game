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
            
            _view.OnBackClicked += ReturnToGeneralSearch;

            _controller = new GenreController
            (
                view: _view,
                apiService: ApiService.Instance,
                config: _config,
                localizationService: LocalizationManager.Instance
            );

            _view.OnGenreClicked += HandleGenreClicked;
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

        private void HandleGenreClicked(int genreId)
        {
            SelectedGenreId = genreId;
            UINavigationMediator.Instance.ReplacePanel(Panels.Genre, Panels.Movies);
        }
        
        private void OnDestroy()
        {
            _view.OnBackClicked -= ReturnToGeneralSearch;
            _view.OnGenreClicked -= HandleGenreClicked;
        }
    }
}