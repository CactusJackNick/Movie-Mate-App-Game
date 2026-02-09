using System;
using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public interface IGenreView
    {
        event Action<int> OnGenreClicked;
        event Action OnBackClicked;
        
        void DisplayGenres(List<GenreViewModel> genres);
    }
}