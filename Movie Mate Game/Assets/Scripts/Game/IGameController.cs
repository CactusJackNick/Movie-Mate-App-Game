using System;

namespace DefaultNamespace.Game
{
    public interface IGameController : IDisposable
    { 
        event Action OnBackButtonRequested;
        void LoadMovies();
    }
}