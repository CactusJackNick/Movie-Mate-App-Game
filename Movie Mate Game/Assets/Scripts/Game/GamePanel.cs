using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Game
{
    public class GamePanel : ABaseUIMediatorComponent
    {
        [SerializeField] private Button _backButton;
        
        public override void Awake()
        {
            base.Awake();
            _backButton.onClick.AddListener(ReturnToMain);
        }

        private void ReturnToMain()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(ReturnToMain);
        }

        public override void Show()
        {
            //action 1 frame before it actually shows
            base.Show();
        }
    }
}