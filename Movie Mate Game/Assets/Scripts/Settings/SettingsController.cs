using DefaultNamespace;
using DefaultNamespace.OptionsSelector;
using UnityEngine;

namespace Settings
{
    public class SettingsController : ISettingsController
    {
        private const string PrefResolutionIndex = "User_Resolution_Index";

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

        public void InitializeSettings()
        {
            InitLanguage();
            InitResolution();
        }
        private void InitLanguage()
        {
            var currentLang = LocalizationManager.Instance.CurrentLanguage;
            
            _languageToggle.Setup(currentLang);
            _languageToggle.OnLanguageChanged += SwitchLanguageSelected;
        }

        private void SwitchLanguageSelected(LocalizationLanguage language)
        {
            LocalizationManager.Instance.CurrentLanguage = language;

            var currentLang = LocalizationManager.Instance.GetCurrentLanguageCode();
            ApiService.Instance.SetLanguage(currentLang);
        }

        private void InitResolution()
        {
            _resolutionSelector.SetupView();
            
            var savedIndex = PlayerPrefs.GetInt(PrefResolutionIndex);
        
            _resolutionSelector.SelectButtonAtIndex(savedIndex);

            _resolutionSelector.OnResolutionChanged += ApplyResolutionSettings;
        }
        
        private void ApplyResolutionSettings(int index)
        {
            var resolutionSize = "w300";
            switch (index)
            {
                case 0:
                    // low
                    resolutionSize = "w300";
                    break;
                case 1:
                    // mid
                    resolutionSize = "w500";
                    break;
                case 2:
                    // high
                    resolutionSize = "original";
                    break;
            }
            
            PlayerPrefs.SetInt(PrefResolutionIndex, index);
            PlayerPrefs.Save();
            
            ApiService.Instance.SetImageResolution(resolutionSize);
            Debug.Log($"Applying resolution {resolutionSize}");
        }
    }
}