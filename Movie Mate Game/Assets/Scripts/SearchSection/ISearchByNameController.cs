using System;

namespace SearchSection
{
    public interface ISearchByNameController : IDisposable
    {
        event Action OnBackRequested;
        event Action<string> OnSearchButtonRequested;
        
        void Initialize();
        void LoadInitialMovies();
        void StartSearch(string search);
        void LoadNextPage();
    }
}