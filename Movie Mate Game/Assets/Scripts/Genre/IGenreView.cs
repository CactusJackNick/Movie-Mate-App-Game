using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace.Genre
{
    public interface IGenreView
    {
        void DisplayGenres(List<GenreViewModel> genres);
    }
}