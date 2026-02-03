using System.Collections.Generic;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public interface IGameView
    {
        void DisplayMovies(List<MovieData> movies);
        void AddMovies(List<MovieData> movies);
        void AssignData(DetailsSuperlistModel data, string director, string actors, string genres);
        void SetPoster(Sprite poster);
        void SetBackdrop(Sprite backdrop);
        void DisplayClues(List<ClueData> clues);
        void UnlockClue(int index);
        void ClearGuessesItems();
    }
}