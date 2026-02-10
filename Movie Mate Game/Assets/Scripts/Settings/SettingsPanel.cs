using DefaultNamespace;
using DefaultNamespace.OptionsSelector;
using Mediator;
using UnityEngine;

namespace Settings
{
    public class SettingsPanel : ABaseUIMediatorComponent
    {
        [SerializeField] private SettingsView _view;
        [SerializeField] private ResolutionSelector _resolutionSelector;
        [SerializeField] private LanguageToggle _languageToggle;
        
        private SettingsController _controller;
        
        public override void Awake()
        {
            base.Awake();
            
            _controller = new SettingsController
                (
                    _view,
                    _languageToggle,
                    _resolutionSelector,
                    ApiService.Instance,
                    LocalizationManager.Instance
                );
            
            _controller.OnGoBackRequested += ReturnToMain;
        }

        public override void Show()
        {
            base.Show();
            _controller.InitializeSettings();
        }

        private void ReturnToMain()
        {
            UINavigationMediator.Instance.ReplacePanel(_panelId, Panels.MainMenu);
        }

        private void OnDestroy()
        {
            if (_controller is null)
            {
                return;
            }
            _controller.OnGoBackRequested -= ReturnToMain;
            _controller.Dispose();
        }
    }
}