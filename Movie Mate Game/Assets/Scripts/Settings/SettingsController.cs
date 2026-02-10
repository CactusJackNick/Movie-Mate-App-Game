using System;
using DefaultNamespace;
using DefaultNamespace.OptionsSelector;
using UnityEngine;

namespace Settings
{
    public class SettingsController : ISettingsController
    {
        private const string PrefResolutionIndex = "User_Resolution_Index";

        private readonly ISettingsView _view;
        private readonly IResolutionSelector _resolutionSelector;
        private readonly ILanguageToggle _languageToggle;
        private readonly IApiService _apiService;
        private readonly ILocalizationService _localizationService;
        
        public SettingsController(ISettingsView view, ILanguageToggle languageToggle,
            IResolutionSelector resolutionSelector, IApiService apiService, ILocalizationService localizationService )
        {
            _view = view;
            _languageToggle = languageToggle;
            _resolutionSelector = resolutionSelector;
            _apiService = apiService;
            _localizationService = localizationService;
            
            _view.OnBackClicked += HandleBackRequest;
        }

        public event Action OnGoBackRequested;

        public void InitializeSettings()
        {
            InitLanguage();
            InitResolution();
        }
        private void InitLanguage()
        {
            var currentLang = _localizationService.CurrentLanguage;
            
            _languageToggle.Setup(currentLang);
            _languageToggle.OnLanguageChanged += SwitchLanguageSelected;
        }

        private void SwitchLanguageSelected(LocalizationLanguage language)
        {
            _localizationService.CurrentLanguage = language;

            var currentLang = _localizationService.GetCurrentLanguageCode();
            _apiService.SetLanguage(currentLang);
        }

        private void InitResolution()
        {
            _resolutionSelector.SetupView();
            
            var savedIndex = PlayerPrefs.GetInt(PrefResolutionIndex, 0);
        
            _resolutionSelector.SelectButtonAtIndex(savedIndex);

            _resolutionSelector.OnResolutionChanged += ApplyResolutionSettings;
        }
        
        private void ApplyResolutionSettings(int index)
        {
            const string resSmall = "w300";
            const string resMid = "w500";
            const string resHigh = "original";
            
            var resolutionSize = "w300";
            
            switch (index)
            {
                case 0:
                    // low
                    resolutionSize = resSmall;
                    break;
                case 1:
                    // mid
                    resolutionSize = resMid;
                    break;
                case 2:
                    // high
                    resolutionSize = resHigh;
                    break;
            }
            
            PlayerPrefs.SetInt(PrefResolutionIndex, index);
            PlayerPrefs.Save();
            
            _apiService.SetImageResolution(resolutionSize);
            Debug.Log($"Applying resolution {resolutionSize}");
        }
        
        private void HandleBackRequest()
        {
            OnGoBackRequested?.Invoke();
        }
        
        public void Dispose()
        {
            _view.OnBackClicked -= HandleBackRequest;

            if (_languageToggle is null)
            {
                return;
            }
            _languageToggle.OnLanguageChanged -= SwitchLanguageSelected;

            if (_resolutionSelector is null)
            {
                return;
            }
            _resolutionSelector.OnResolutionChanged -= ApplyResolutionSettings;
        }
    }
}