using System.Collections.Generic;

namespace DefaultNamespace.Game
{
    public interface ISearchController
    {
        void StartSearch(string search);
        void LoadNextPage();
        void ResetSearchState();
        void SetFilter(HashSet<int> guessedIds);
        void Dispose();
    }
}