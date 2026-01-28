using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public interface IMovieView
    {
        void DisplayMovies(List<MovieData> movies);
        void ShowNoMoviesText(bool isActive);
    }
}