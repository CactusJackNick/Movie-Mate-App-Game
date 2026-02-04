using System;

namespace DefaultNamespace.Game
{
    public interface IGameController : IDisposable
    {
        void LoadMovies();
    }
}