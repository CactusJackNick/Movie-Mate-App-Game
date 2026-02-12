using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
    public class MovieDetailsController : IMovieDetailsController
    {
        private readonly IMovieDetailsView _view;
        private readonly IApiService _api;

        public MovieDetailsController(IMovieDetailsView view, IApiService api)
        {
            _view = view;
            _api = api;

            _view.backButtonPressed += OnBackButtonRequest;
        }
        public event Action OnBackButtonRequested;

        public void LoadMovieInfo(int movieId)
        {
            LoadDetailsAsync(movieId).Forget();
        }

        private async UniTask LoadDetailsAsync(int movieId)
        {
            LoadingPanel.Instance.Show();
            
            var details = await _api.GetMovieDetailsAsync(movieId);

            if (details == null)
            {
                return;
            }

            var directorName = "Unknown";
            if (details.Credits.Crew != null)
            {
                foreach (var person in details.Credits.Crew)
                {
                    if (person.Job == "Director")
                    {
                        directorName = person.NameCrew;
                        break;
                    }
                }
            }

            var genresText = "";
            var genresList = new List<string>();
            if (details.Genres is { Count: > 0 })
            {
                foreach (var genre in details.Genres)
                {
                    genresList.Add(genre.Name);
                } 
                
                genresText = string.Join(", ", genresList);
            }
            
            _view.DisplayData(details, directorName, genresText);

            if (!string.IsNullOrEmpty(details.PosterPath))
            {
                var sprite = await _api.GetMovieImageAsync(details.PosterPath);
                _view.SetPoster(sprite);
            }
            
            LoadingPanel.Instance.Hide();
        }

        private void OnBackButtonRequest()
        {
            OnBackButtonRequested?.Invoke();
        }

        public void Dispose()
        {
            _view.backButtonPressed -= OnBackButtonRequest;
        }
    }
}