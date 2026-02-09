using System;
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
        private readonly ILocalizationService _localizationService;
        private readonly Dictionary<int, GenreViewModel> _genreButtonsList = new();
        
        private string _lastLoadedLanguage = "";
        
        public GenreController(IGenreView view, IApiService apiService, 
            GenreIconsConfig config, ILocalizationService localizationService)
        {
            _view = view;
            _apiService = apiService;
            _config = config;
            _localizationService = localizationService;

            _view.OnGenreClicked += HandleGenreClicked;
        }
        
        public event Action<int> OnGenreSelected;        

        public async UniTask LoadGenresAsync()
        {
            CheckCurrentLanguage();
            
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

        private void HandleGenreClicked(int genreId)
        {
            OnGenreSelected?.Invoke(genreId);
        }

        private void CheckCurrentLanguage()
        {
            var currentLanguage = _localizationService.GetCurrentLanguageCode();

            if (_lastLoadedLanguage is null ||
                _lastLoadedLanguage != currentLanguage)
            {
                _lastLoadedLanguage = currentLanguage;
                _genreButtonsList.Clear();
            }
        }

        public void Dispose()
        {
            _view.OnGenreClicked -= HandleGenreClicked;
        }
    }
}