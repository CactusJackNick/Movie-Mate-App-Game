using System;

namespace Settings
{
    public interface ISettingsController : IDisposable
    { 
        event Action OnGoBackRequested;
        
        void InitializeSettings();
    }
}