namespace DefaultNamespace
{
    public interface ILocalizationService
    {
        LocalizationLanguage CurrentLanguage{get; set;} 
        
        string GetCurrentLanguageCode();
    }
}