using System.Collections.Generic;
using DefaultNamespace.Models;

namespace DefaultNamespace.Game
{
    public interface IGameView
    {
        void DisplayMovies(List<MovieData> movies);
        void AddMovies(List<MovieData> movies);
        void DisplayClues(List<ClueData> clues);
        void UnlockClue(int index);
        void ClearGuessesItems();
        void ShowFeedbackResult(GuessResultModel result);
    }
}