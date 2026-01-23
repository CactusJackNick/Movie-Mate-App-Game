using Mediator;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class GamePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private GameView _view;
        
        private GameController _controller;
        
        public override void Awake()
        {
            base.Awake();
            
            _view.OnBackButtonPressed += ReturnToMain;

            _controller = new GameController(_view, ApiService.Instance);
        }
        public override void Show()
        {
            base.Show();
            _controller.LoadMovies();
        }
        
        private void ReturnToMain()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
        }

        private void OnDestroy()
        {
            _view.OnBackButtonPressed -= ReturnToMain;
            _controller?.Dispose();
        }
    }
}