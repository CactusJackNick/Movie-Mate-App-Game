using System;

namespace DefaultNamespace.OptionsSelector
{
    public interface ILanguageToggle
    {
        event Action<LocalizationLanguage> OnLanguageChanged;

        void Setup(LocalizationLanguage currentLang);
    }
}