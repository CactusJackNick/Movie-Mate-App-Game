using System;
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

        private ApiService()
        {
            _instance = this;
        }
        
        public static IApiService Instance => _instance ??= new ApiService();

        
        public async UniTask<MovieListResponse> GetPopularMoviesAsync()
        {
            var url = $"{BaseURL}/movie/popular";
            
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
            var url = $"{BaseURL}/genre/movie/list";
            
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
    }
}