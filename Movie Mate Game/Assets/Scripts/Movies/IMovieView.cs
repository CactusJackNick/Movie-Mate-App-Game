using System;
using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    public interface IMovieView
    { 
        event Action OnBackButtonPressed;
        event Action<MovieData> DetailsButtonClicked;
        
        void DisplayMovies(List<MovieData> movies);
        void AddMovies(List<MovieData> movies);
        void ClearItems();
    }
}