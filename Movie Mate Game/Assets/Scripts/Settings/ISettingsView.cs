using System;

namespace Settings
{
    public interface ISettingsView
    {
        event Action OnBackClicked;
    }
}