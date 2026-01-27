using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public class MoviesController : IMovieController
    {
        public List<MovieData> filteredList = new();
        
        private readonly IMovieView _movieView;
        private readonly IApiService  _apiService;
        
        
        public MoviesController(IMovieView movieView, IApiService apiService)
        {
            _movieView = movieView;
            _apiService = apiService;
        }

        public void LoadFilteredMovies(int targetId)
        {
            LoadMoviesAsync(targetId).Forget();
        }

        private async UniTask LoadMoviesAsync(int targetId)
        {
            var response = await _apiService.GetPopularMoviesAsync();
            
            if (response == null || 
                response.Results == null)
            {
                return;
            } 
            
            foreach (var movieData in response.Results)
            {
                foreach (var genreId in movieData.Genre_Ids)
                {
                    if(genreId == targetId)
                    {
                        filteredList.Add(movieData);
                    }

                    _movieView.ShowNoMoviesText(filteredList.Count <= 0);
                }
            }
            
            _movieView.DisplayMovies(filteredList);
        }
    }
}