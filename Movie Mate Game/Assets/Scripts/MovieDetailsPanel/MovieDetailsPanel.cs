using Mediator;
using UnityEngine;

namespace DefaultNamespace
{
    public class MovieDetailsPanel : ABaseUIMediatorComponent
    {
        public static int TargetMovieId;
        public static Panels PreviousPanel;
        
        [SerializeField] private MovieDetailsView _view;
        
        private MovieDetailsController _controller;

        public override void Awake()
        {
            base.Awake();
            
            _view.backButtonPressed += GoBack;

            _controller = new MovieDetailsController(_view, ApiService.Instance);
        }

        public override void Show()
        {
            base.Show();
            _controller.LoadMovieInfo(TargetMovieId);
        }

        private void GoBack()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, PreviousPanel);
        }

        private void OnDestroy()
        {
            _view.backButtonPressed -= GoBack;
        }
    }
}