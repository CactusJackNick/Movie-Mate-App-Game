using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class GameController : IGameController
    {
        private readonly IGameView _view;
        private readonly IApiService _apiService;

        public GameController(IGameView view, IApiService apiService)
        {
            _view = view;
            _apiService = apiService;
        }

        public void LoadMovies()
        {
            LoadMoviesAsync().Forget();
        }

        private async UniTask LoadMoviesAsync()
        {
            try
            {
                var listResponse = await _apiService.GetPopularMoviesAsync();

                if (listResponse.Results is { Count: > 0 })
                {
                    _view.DisplayMovies(listResponse.Results);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading movies: {ex.Message} ");
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            //this.Dispose();
        }
    }
    
}