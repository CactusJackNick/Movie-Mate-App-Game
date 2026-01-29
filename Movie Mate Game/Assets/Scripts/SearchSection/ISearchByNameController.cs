namespace SearchSection
{
    public interface ISearchByNameController
    {
        void Initialize(ISearchByNameView view);
        void LoadInitialMovies();
        void StartSearch(string search);
        void LoadNextPage();
    }
}