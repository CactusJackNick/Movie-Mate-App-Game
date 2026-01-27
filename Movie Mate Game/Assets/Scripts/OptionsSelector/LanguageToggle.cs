using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.OptionsSelector
{
    public class LanguageToggle : MonoBehaviour
    {
        [SerializeField] private Toggle _englishToggle;
        [SerializeField] private Toggle _russianToggle;

        private void Awake()
        {
            var currentLang = LocalizationManager.Instance.CurrentLanguage;
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
                SwitchLanguageSelected(LocalizationLanguage.English);
            }
        }
        
        private void OnRussianSelected(bool isOn)
        {
            if (isOn)
            {
                SwitchLanguageSelected(LocalizationLanguage.Russian);
            }
        }

        public void SwitchLanguageSelected(LocalizationLanguage language)
        {
            LocalizationManager.Instance.CurrentLanguage = language;

            var currentLang = LocalizationManager.Instance.GetCurrentLanguageCode();
            ApiService.Instance.SetLanguage(currentLang);
        }

        private void OnDisable()
        {
            _englishToggle.onValueChanged.RemoveListener(OnEnglishSelected);
            _russianToggle.onValueChanged.RemoveListener(OnRussianSelected);
        }
    }
}