using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class ApiService : IApiService
    {
        private static IApiService _instance;
        
        private const string BASE_URL = "https://api.themoviedb.org/3";
        private const string BEARER_TOKEN = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJmOTMxNzQ1ZmY5Y2QyZjkzNmI1YWJmN2RkZmY3YzIxMyIsIm5iZiI6MTc2OTEwOTUyMC42MzY5OTk4LCJzdWIiOiI2OTcyNzgxMGM4MzMwOWU4OTQ4OTM5NzAiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.zPgF6CKua7OmFgvU_NPfiuDc9s6542V5N-DyuW52DcA";

        private string _currentRes = "w342";
        private string _currentLang = "en-US";
        
        private Dictionary<string, Sprite> _spriteCache = new();

        private ApiService()
        {
            _instance = this;
        }
        
        public static IApiService Instance => _instance ??= new ApiService();

        public void SetImageResolution(string newResolution)
        {
            if (_currentRes == newResolution)
            {
                return;
            }
            
            _currentRes =  newResolution;
            _spriteCache.Clear();
        }

        public void SetLanguage(string newLanguage)
        {
            if (_currentLang == newLanguage)
            {
                return;
            }
            
            _currentLang = newLanguage;
            _spriteCache.Clear();
        }
        
        public async UniTask<MovieListResponse> SearchMoviesAsync(string query, int page)
        {
            var url = $"{BASE_URL}/search/movie?language={_currentLang}&page={page}&query={query}";
            return await SendResponseDataAsync<MovieListResponse>(url);
        }
        
        public async UniTask<MovieListResponse> GetDiscoverMoviesAsync(int page)
        {
            var url = $"{BASE_URL}/discover/movie?language={_currentLang}&page={page}";
            return await SendResponseDataAsync<MovieListResponse>(url); // Returns 40k
        }
        
        public async UniTask<MovieListResponse> GetPopularMoviesAsync(int page)
        {
            var url = $"{BASE_URL}/movie/popular?language={_currentLang}&page={page}";

            var responseData = await SendResponseDataAsync<MovieListResponse>(url);
            return responseData; // returns 20k
        }

        public async UniTask<MovieListResponse> GetMoviesByGenreAsync(int genreId, int page)
        {
            var url = $"{BASE_URL}/discover/movie?with_genres={genreId}&page={page}&language={_currentLang}";
            return await SendResponseDataAsync<MovieListResponse>(url);
        } 
        
        public async UniTask<GenresListResponse> GetGenreListAsync()
        {
            var url = $"{BASE_URL}/genre/movie/list?language={_currentLang}";
            return await SendResponseDataAsync<GenresListResponse>(url);
        }
        
        public async UniTask<Sprite> GetMovieImageAsync(string posterPath)
        {
            if (string.IsNullOrEmpty(posterPath)) 
            {
                return null;
            }
            
            if (_spriteCache.TryGetValue(posterPath, out var cachedSprite))
            {
                if (cachedSprite != null)
                {
                    return cachedSprite;
                }
                _spriteCache.Remove(posterPath); 
            }
            
            var tex = await GetMovieTexture(posterPath);
            if (tex == null)
            {
                return null;
            }
            
            var newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            _spriteCache[posterPath] = newSprite;
            
            return newSprite;
        }

        private async UniTask<Texture2D> GetMovieTexture(string posterPath)
        {
            var url = $"https://image.tmdb.org/t/p/{_currentRes}{posterPath}?language={_currentLang}";
            
            using var request = UnityWebRequestTexture.GetTexture(url);
            request.SetRequestHeader("Authorization", $"Bearer {BEARER_TOKEN}");
            request.SetRequestHeader("accept", "application/json");

            try
            {
                await request.SendWebRequest();
            }
            catch (Exception)
            {
                Debug.LogWarning($"[ApiService] Image not found (404) or connection error: {url}");
                return null;
            }
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                return null;
            }

            var texture = DownloadHandlerTexture.GetContent(request);
            return texture;
        }
        
        public async UniTask<DetailsSuperlistModel> GetMovieDetailsAsync(int movieId)
        {
            var url = $"{BASE_URL}/movie/{movieId}?language={_currentLang}&append_to_response=credits";
            return await SendResponseDataAsync<DetailsSuperlistModel>(url);
        }
        
        private static async UniTask<T> SendResponseDataAsync<T>(string url)
        {
            using var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer {BEARER_TOKEN}");
            request.SetRequestHeader("accept", "application/json");
            
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                throw new Exception(request.error);
            }
            
            var json = request.downloadHandler.text;
            var data = JsonConvert.DeserializeObject<T>(json);
            
            return data;
        }
    }
}