using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace.Game
{
    public interface IMoviePickService
    {
        UniTask<DetailsSuperlistModel> GetValidGameMovieAsync();
    }
}