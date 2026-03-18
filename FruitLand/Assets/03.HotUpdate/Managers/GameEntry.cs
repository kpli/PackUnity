using AOT;
using UnityEngine;

namespace HotUpdate
{
    public class GameEntry : IGameEntry
    {
        private bool _isRunning;

        public void Initialize()
        {
            Debug.Log("[HotUpdate] GameEntry initialized");
            _isRunning = false;
        }

        public void StartGame()
        {
            Debug.Log("[HotUpdate] Game started!");
            _isRunning = true;
        }

        public void Update(float deltaTime)
        {
            if (!_isRunning) return;
        }

        public void Shutdown()
        {
            Debug.Log("[HotUpdate] GameEntry shutdown");
            _isRunning = false;
        }
    }
}
