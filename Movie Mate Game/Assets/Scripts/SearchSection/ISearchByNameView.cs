using System;

namespace SearchSection
{
    public interface ISearchByNameView
    {
        event Action OnBackButtonPressed;
        event Action<string> OnSearchButtonPressed;
        
        void SetupViewInitialState();
    }
}