using DefaultNamespace;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsPanel : ABaseUIMediatorComponent
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
    }
}