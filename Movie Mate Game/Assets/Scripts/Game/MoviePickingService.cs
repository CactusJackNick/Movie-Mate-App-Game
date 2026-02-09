using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public class MoviePickingService : IMoviePickService
    {
        private readonly IApiService _apiService;

        public MoviePickingService(IApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async UniTask<DetailsSuperlistModel> GetValidGameMovieAsync()
        {
            LoadingPanel.Instance.Show();
            
            const int maxPageSize = 100;
            var maxAttempts = 10;
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                attempts++;
                var randomPageNumber = Random.Range(1, maxPageSize);
                var listResponse = await _apiService.GetPopularMoviesAsync(randomPageNumber);

                if (listResponse.Results == null || listResponse.Results.Count == 0)
                {
                    continue;
                }
                
                var candidates = ShuffleListRandomly(listResponse.Results);

                foreach (var candidate in candidates)
                {
                    if (string.IsNullOrEmpty(candidate.backdrop_path))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(candidate.poster_path))
                    {
                        continue;
                    }

                    if (candidate.Genre_Ids == null || candidate.Genre_Ids.Count == 0)
                    {
                        continue;
                    }

                    var details = await _apiService.GetMovieDetailsAsync(candidate.Id);

                    if (ValidateMovie(details))
                    {
                        Debug.Log($"Found valid game movie: {details.Title}");
                        return details;
                    }
                }
            }
            
            LoadingPanel.Instance.Hide();
            return null;
        }
        
        private bool ValidateMovie(DetailsSuperlistModel movie)
        {
            if (string.IsNullOrEmpty(movie.Tagline))
            {
                return false;
            }

            var hasDirector = false;
            foreach (var person in movie.Credits.Crew)
            {
                if (person.Job == "Director")
                {
                    hasDirector = true;
                }
            }
           
            if (!hasDirector)
            {
                return false;
            }

            if (movie.Credits.Cast == null || movie.Credits.Cast.Count < 3)
            {
                return false;
            }
            
            return true;
        }

        private List<MovieData> ShuffleListRandomly(List<MovieData> inputList)
        {    
            //modern Fischer-Yates shuffle developed by Richard Durstenfeld
            var tempList = new List<MovieData>(inputList);
            var n = tempList.Count;

            for (int i = n - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                
                var temp = tempList[i];
                tempList[i] = tempList[j];
                tempList[j] = temp;
            }
        
            return tempList;
        }
    }
}