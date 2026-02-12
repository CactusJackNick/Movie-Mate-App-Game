using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace
{
    public class MovieDetailsController : IMovieDetailsController
    {
        private readonly IMovieDetailsView _view;
        private readonly IApiService _api;
        private readonly ILoadingPanel _loadingPanel;

        public MovieDetailsController(IMovieDetailsView view, IApiService api, ILoadingPanel loadingPanel)
        {
            _view = view;
            _api = api;
            _loadingPanel = loadingPanel;

            _view.backButtonPressed += OnBackButtonRequest;
        }
        public event Action OnBackButtonRequested;

        public void LoadMovieInfo(int movieId)
        {
            LoadDetailsAsync(movieId).Forget();
        }

        private async UniTask LoadDetailsAsync(int movieId)
        {
            _loadingPanel.Show();
            
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
            
            _loadingPanel.Hide();
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