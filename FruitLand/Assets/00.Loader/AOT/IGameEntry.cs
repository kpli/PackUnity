using System;

namespace AOT
{
    public interface IGameEntry
    {
        void Initialize();
        void StartGame();
        void Update(float deltaTime);
        void Shutdown();
    }
}
