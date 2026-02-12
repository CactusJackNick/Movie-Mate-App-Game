using System;

namespace DefaultNamespace
{
    public interface IMovieDetailsController : IDisposable
    {
        event Action OnBackButtonRequested;
        void LoadMovieInfo(int movieId);
    }
}