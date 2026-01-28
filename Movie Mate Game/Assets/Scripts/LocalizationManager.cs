using UnityEngine;

public enum LocalizationLanguage
{
    English,
    Russian
}

public class LocalizationManager
{
    private static LocalizationManager _instance;
    public static LocalizationManager Instance => _instance ??= new LocalizationManager();

    private const string ManagerSaveKey = "active-language";

    public LocalizationLanguage CurrentLanguage
    {
        get => (LocalizationLanguage)PlayerPrefs.GetInt(ManagerSaveKey, 0);
        set => PlayerPrefs.SetInt(ManagerSaveKey, (int)value);
    }
        
    public string GetCurrentLanguageCode()
    {
        return LanguageToCode(CurrentLanguage);
    }
        
    private string LanguageToCode(LocalizationLanguage lang)
    {
        return lang switch
        {
            LocalizationLanguage.English => "en-US",
            LocalizationLanguage.Russian => "ru-RU",
            _ => "en"
        };
    }
}