using System;
using Cysharp.Threading.Tasks;

namespace DefaultNamespace.Game
{
    public interface IResultsView
    {
        event Action OnNewGameButtonPressed;
        event Action OnExitButtonPressed;
        UniTask ShowResultsAsync(bool hasWon, string targetMovie);
    }
}