using DefaultNamespace.Models;

namespace DefaultNamespace.Game
{
    public interface IFeedbackService
    {
        public GuessResultModel EvaluateGuess(DetailsSuperlistModel guess, DetailsSuperlistModel target);
    }
}