using DefaultNamespace.OptionsSelector;

namespace Settings
{
    public class SettingsController : ISettingsController
    {
        private readonly ISettingsView _view;
        private ResolutionSelector _resolutionSelector;
        private LanguageToggle _languageToggle;
        
        public SettingsController(ISettingsView view)
        {
            _view = view;
        }

        public void Configure(ResolutionSelector resolutionSelector, LanguageToggle languageToggle)
        {
            _resolutionSelector = resolutionSelector;
            _languageToggle = languageToggle;
        }

        public void InitResButtons()
        {
            _resolutionSelector.SetupView();
            _resolutionSelector.InitializeSavedState();
        }
    }
}