using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IApiService
    {
        UniTask<MovieListResponse> GetPopularMoviesAsync();
        UniTask<GenresListResponse> GetGenreListAsync();
        UniTask<Sprite> GetMovieImage(string posterPath);

    }
}