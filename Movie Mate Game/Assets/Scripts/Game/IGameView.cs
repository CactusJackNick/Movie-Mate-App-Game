using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace.Game
{
    public interface IGameView
    {
        void DisplayMovies(List<MovieData> movies);
    }
}