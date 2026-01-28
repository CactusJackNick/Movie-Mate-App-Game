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
        
        private const string BaseURL = "https://api.themoviedb.org/3";
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
        
        public async UniTask<MovieListResponse> GetPopularMoviesAsync()
        {
            var url = $"{BaseURL}/movie/popular?language={_currentLang}";
            
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
            var data = JsonConvert.DeserializeObject<MovieListResponse>(json);
            
            return data;
        }

        public async UniTask<GenresListResponse> GetGenreListAsync()
        {
            var url = $"{BaseURL}/genre/movie/list?language={_currentLang}";
            
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
            var data = JsonConvert.DeserializeObject<GenresListResponse>(json);
            
            return data;
        }
        
        public async UniTask<Sprite> GetMovieImageAsync(string posterPath)
        {
            var tex = await GetMovieTexture(posterPath);
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
            
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                throw new Exception(request.error);
            }

            var texture = DownloadHandlerTexture.GetContent(request);
            return texture;
        }
    }
}