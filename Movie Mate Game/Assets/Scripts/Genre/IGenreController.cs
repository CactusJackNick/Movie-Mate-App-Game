using System;
using Cysharp.Threading.Tasks;

namespace Genre
{
    public interface IGenreController : IDisposable
    {
        event Action OnCloseRequested;
        event Action<int> OnGenreSelected;
        UniTask LoadGenresAsync();
    }
}