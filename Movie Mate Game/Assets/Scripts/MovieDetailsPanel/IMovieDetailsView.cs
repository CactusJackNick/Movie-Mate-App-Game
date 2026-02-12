using System;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IMovieDetailsView
    {
        event Action backButtonPressed;
        
        void DisplayData(DetailsSuperlistModel data, string director, string genres);
        void SetPoster(Sprite sprite);
        void ClearView();
    }
}