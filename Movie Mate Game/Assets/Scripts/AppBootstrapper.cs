using UnityEngine;

public static class AppBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeApp()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}