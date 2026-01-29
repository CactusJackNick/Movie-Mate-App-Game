using DefaultNamespace.SearchSection;
using Mediator;
using UnityEngine;

namespace DefaultNamespace
{
    public class SearchPanel : ABaseUIMediatorComponent
    {
        [SerializeField] private SearchView _view;
        
        public override void Awake()
        {
            base.Awake();
            
            _view.OnBackClicked += ReturnToMain;
            _view.OnFindMoviesClicked += OpenSeachByNamePanel;
            _view.OnGenreClicked += OpenSearchByGenrePanel;
        }

        public override void Show()
        {
            base.Show();
        }

        private void ReturnToMain()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
        }

        private void OpenSeachByNamePanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.SeachByNamePanel);
        }

        private void OpenSearchByGenrePanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.Genre);
        }

        private void OnDestroy()
        {
            _view.OnBackClicked -= ReturnToMain;
            _view.OnFindMoviesClicked -= OpenSeachByNamePanel;
            _view.OnGenreClicked -= OpenSearchByGenrePanel;
        }
    }
}