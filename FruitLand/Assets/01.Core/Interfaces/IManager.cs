using System;

namespace Core.Interfaces
{
    public interface IManager
    {
        void Initialize();
        void Update(float deltaTime);
        void Shutdown();
    }
}
