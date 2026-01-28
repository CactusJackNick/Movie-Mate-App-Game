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

            _view.OnBackClicked += ReturnToMain;
            
            _controller = new SettingsController(_view);
            _controller.Configure(_resolutionSelector,  _languageToggle);
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
            _view.OnBackClicked -= ReturnToMain;
        }
    }
}