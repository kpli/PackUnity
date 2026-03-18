using System;
using System.Reflection;
using UnityEngine;

namespace AOT
{
    public static class HybridCLRLoader
    {
        private static IGameEntry _gameEntry;

        public static bool IsLoaded => _gameEntry != null;

        public static void LoadHotUpdate()
        {
            var hotUpdateAssembly = Assembly.Load("HotUpdate");
            if (hotUpdateAssembly == null)
            {
                Debug.LogError("[HybridCLRLoader] Failed to load HotUpdate assembly");
                return;
            }

            var gameEntryType = hotUpdateAssembly.GetType("HotUpdate.GameEntry");
            if (gameEntryType == null)
            {
                Debug.LogError("[HybridCLRLoader] GameEntry type not found in HotUpdate assembly");
                return;
            }

            _gameEntry = Activator.CreateInstance(gameEntryType) as IGameEntry;
            if (_gameEntry == null)
            {
                Debug.LogError("[HybridCLRLoader] Failed to create GameEntry instance");
                return;
            }

            Debug.Log("[HybridCLRLoader] HotUpdate assembly loaded successfully");
            _gameEntry.Initialize();
        }

        public static void StartGame()
        {
            _gameEntry?.StartGame();
        }

        public static void Update(float deltaTime)
        {
            _gameEntry?.Update(deltaTime);
        }

        public static void Shutdown()
        {
            _gameEntry?.Shutdown();
            _gameEntry = null;
        }
    }
}
