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
            
            _controller = new MovieDetailsController(_view, ApiService.Instance, LoadingPanel.Instance);

            _controller.OnBackButtonRequested += GoBack;
        }

        public override void Show()
        {
            base.Show();
            _view.ClearView();
            _controller.LoadMovieInfo(TargetMovieId);
        }

        private void GoBack()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, PreviousPanel);
        }

        private void OnDestroy()
        {
            _controller.OnBackButtonRequested -= GoBack;
            _controller?.Dispose();
        }
    }
}