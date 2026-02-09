using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public interface IMovieController : IDisposable
    {
        event Action OnCloseButtonRequested;
        event Action<MovieData> OnDetailsRequested;

        UniTask LoadNextPage();
        UniTask LoadFilteredMovies(int targetId);
    }
}