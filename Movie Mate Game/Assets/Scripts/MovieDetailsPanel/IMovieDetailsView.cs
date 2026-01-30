using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IMovieDetailsView
    {
        void DisplayData(DetailsSuperlistModel data, string director, string genres);
        void SetPoster(Sprite sprite);
    }
}