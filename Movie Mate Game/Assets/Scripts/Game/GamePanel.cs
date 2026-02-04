using Cysharp.Threading.Tasks;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class GamePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private GameView _view;
        [SerializeField] private ScrollRect _scrollRect;
        
        private GameController _controller;
        
        public override void Awake()
        {
            base.Awake();

            _controller = new GameController(_view, ApiService.Instance);
            
            _view.OnBackButtonPressed += ReturnToMain;
            _view.OnInputPressed += OnSubmit;
            _view.OnGuessSelected += OnPlayerGuess;
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        public override void Show()
        {
            base.Show();
            
            _scrollRect.verticalNormalizedPosition = 1f;

            _controller.LoadMovies();
        }
        
        private void OnScroll(Vector2 pos)
        {
            if (pos.y < 0.1f)
            {
                _controller.LoadNextPageGuesses();
            }
        }
        
        private void ReturnToMain()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
            _view.ClearGuessesItems();
        }
        
        private void OnSubmit(string text)
        {
            _controller.StartSearch(text);
        }
        
        private void OnPlayerGuess(int movieId)
        {
            _controller.ProcessPlayerGuess(movieId).Forget();
        }

        private void OnDestroy()
        {
            _view.OnBackButtonPressed -= ReturnToMain;
            _view.OnInputPressed -= OnSubmit;
            _view.OnGuessSelected -= OnPlayerGuess;
            _scrollRect.onValueChanged.RemoveListener(OnScroll);
            _controller?.Dispose();
        }
    }
}