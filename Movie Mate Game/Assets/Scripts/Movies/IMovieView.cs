using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public interface IMovieView
    {
        void DisplayMovies(List<MovieData> movies);
        void AddMovies(List<MovieData> movies);
        void ClearItems();
    }
}