using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public interface IApiService
    {
        UniTask<MovieListResponse> GetPopularMoviesAsync();
        UniTask<GenresListResponse> GetGenreListAsync();
    }
}