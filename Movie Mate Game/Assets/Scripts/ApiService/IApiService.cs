using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IApiService
    {
        UniTask<MovieListResponse> GetPopularMoviesAsync(int page);
        UniTask<MovieListResponse> GetDiscoverMoviesAsync(int page);
        UniTask<GenresListResponse> GetGenreListAsync();
        UniTask<MovieListResponse> GetMoviesByGenreAsync(int genreId, int page);
        UniTask<Sprite> GetMovieImageAsync(string posterPath);
        UniTask<MovieListResponse> SearchMoviesAsync(string query, int page);
        UniTask<DetailsSuperlistModel> GetMovieDetailsAsync(int movieId);
        void SetImageResolution(string newResolution);
        void SetLanguage(string newLanguage);
    }
}