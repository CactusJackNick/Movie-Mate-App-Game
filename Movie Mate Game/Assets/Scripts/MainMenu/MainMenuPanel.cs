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
        [SerializeField] private Button _findMoviesButton;

        public override void Awake()
        {
            base.Awake();
            _startGameButton.onClick.AddListener(OpenGamePanel);
            _settingsButton.onClick.AddListener(OpenSettingsPanel);
            _findMoviesButton.onClick.AddListener(OpenSearchPanel);
        }

        private void OpenGamePanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId,Panels.Game);
        }

        private void OpenSettingsPanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId,Panels.Settings);
        }
        
        private void OpenSearchPanel()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId,Panels.SeachPanel);
        }

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveListener(OpenSettingsPanel);
            _startGameButton.onClick.RemoveListener(OpenGamePanel);
            _findMoviesButton.onClick.RemoveListener(OpenSearchPanel);
        }
    }
}