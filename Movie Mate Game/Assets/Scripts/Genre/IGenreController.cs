using Cysharp.Threading.Tasks;

namespace Genre
{
    public interface IGenreController
    {
        UniTask LoadGenresAsync();
    }
}