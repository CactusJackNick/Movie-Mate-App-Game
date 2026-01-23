using DefaultNamespace;
using Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuPanel : ABaseUIMediatorComponent
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _startGameButton;

        public override void Awake()
        {
            base.Awake();
            _startGameButton.onClick.AddListener(OpenGamePanel);
            _settingsButton.onClick.AddListener(OpenSettingsPanel);
        }

        private void OpenGamePanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId,Panels.Game);
            Debug.Log("opened game panel");
        }

        private void OpenSettingsPanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId,Panels.Settings);
        }

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveListener(OpenSettingsPanel);
            _startGameButton.onClick.RemoveListener(OpenGamePanel);
        }
    }
}