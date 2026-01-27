using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Models;
using Genre;

namespace DefaultNamespace.Genre
{
    public class GenreController : IGenreController
    {
        private readonly IGenreView _view;
        private readonly IApiService _apiService;
        private readonly GenreIconsConfig _config;
        private readonly Dictionary<int, GenreViewModel> _genreButtonsList = new();
        
        public GenreController(IGenreView view, IApiService apiService, GenreIconsConfig config)
        {
            _view = view;
            _apiService = apiService;
            _config = config;
        }

        public void LoadGenres()
        {
            LoadGenresAsync().Forget();
        }

        private async UniTask LoadGenresAsync()
        {
            var response = await _apiService.GetGenreListAsync();
            
            foreach (var dto in response.Genres)
            {
                if (!_genreButtonsList.ContainsKey(dto.Id))
                {
                    var genreViewModel = new GenreViewModel
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Icon = _config.GetIconFromId(dto.Id)
                    };
                    
                    _genreButtonsList.Add(dto.Id, genreViewModel);
                }
            }
            
            _view.DisplayGenres(_genreButtonsList.Values.ToList());
        }
    }
}