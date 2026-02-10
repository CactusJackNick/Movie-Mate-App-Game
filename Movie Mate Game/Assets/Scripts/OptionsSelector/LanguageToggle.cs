using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.OptionsSelector
{
    public class LanguageToggle : MonoBehaviour, ILanguageToggle
    {
        [SerializeField] private Toggle _englishToggle;
        [SerializeField] private Toggle _russianToggle;

        public event Action<LocalizationLanguage> OnLanguageChanged;
        
        public void Setup(LocalizationLanguage currentLang)
        {
            var isRussian = currentLang == LocalizationLanguage.Russian;
            
            _englishToggle.SetIsOnWithoutNotify(!isRussian);
            _russianToggle.SetIsOnWithoutNotify(isRussian);
            
            _englishToggle.onValueChanged.AddListener(OnEnglishSelected);
            _russianToggle.onValueChanged.AddListener(OnRussianSelected);
        }
        
        private void OnEnglishSelected(bool isOn)
        {
            if (isOn)
            {
                OnLanguageChanged?.Invoke(LocalizationLanguage.English);
            }
        }
        
        private void OnRussianSelected(bool isOn)
        {
            if (isOn)
            {
                OnLanguageChanged?.Invoke(LocalizationLanguage.Russian);
            }
        }

        private void OnDisable()
        {
            _englishToggle.onValueChanged.RemoveListener(OnEnglishSelected);
            _russianToggle.onValueChanged.RemoveListener(OnRussianSelected);
        }
    }
}