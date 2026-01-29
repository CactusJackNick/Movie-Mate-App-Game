using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IApiService
    {
        UniTask<MovieListResponse> GetPopularMoviesAsync(int page);
        UniTask<GenresListResponse> GetGenreListAsync();
        UniTask<Sprite> GetMovieImageAsync(string posterPath);
        UniTask<MovieListResponse> SearchMoviesAsync(string query, int page);
        void SetImageResolution(string newResolution);
        void SetLanguage(string newLanguage);
    }
}