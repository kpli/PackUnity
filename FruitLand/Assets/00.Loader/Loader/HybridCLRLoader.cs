using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AOT
{
    public static class HybridCLRLoader
    {
        private static HybridCLRConfig _config;
        private static readonly Dictionary<string, Assembly> _loadedAssemblies = new();
        private static IGameEntry _gameEntry;

        public static bool IsLoaded => _gameEntry != null;

        public static void Initialize(HybridCLRConfig config)
        {
            _config = config;
            Debug.Log("[HybridCLRLoader] Initialized with config");
        }

        public static void LoadAOTAssemblies()
        {
            if (_config == null)
            {
                Debug.LogError("[HybridCLRLoader] Config not set, call Initialize first");
                return;
            }

            Debug.Log("[HybridCLRLoader] Loading AOT assemblies...");
            foreach (var assemblyName in _config.aotAssemblies)
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        _loadedAssemblies[assemblyName] = assembly;
                        Debug.Log($"[HybridCLRLoader] AOT assembly loaded: {assemblyName}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[HybridCLRLoader] Failed to load AOT assembly {assemblyName}: {e.Message}");
                }
            }
        }

        public static void LoadHotUpdateAssemblies()
        {
            if (_config == null || !_config.autoLoadHotUpdate)
            {
                Debug.Log("[HybridCLRLoader] Hot update loading skipped");
                return;
            }

            Debug.Log("[HybridCLRLoader] Loading HotUpdate assemblies...");
            foreach (var assemblyName in _config.hotUpdateAssemblies)
            {
                LoadHotUpdateAssembly(assemblyName);
            }
        }

        private static void LoadHotUpdateAssembly(string assemblyName)
        {
            try
            {
                string dllPath = Path.Combine(Application.streamingAssetsPath, _config.hotUpdateDllPath, $"{assemblyName}.dll");
                
                if (!File.Exists(dllPath))
                {
                    Debug.LogWarning($"[HybridCLRLoader] DLL not found at {dllPath}, trying default load");
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        _loadedAssemblies[assemblyName] = assembly;
                        Debug.Log($"[HybridCLRLoader] HotUpdate assembly loaded (default): {assemblyName}");
                    }
                    return;
                }

                byte[] dllBytes = File.ReadAllBytes(dllPath);
                var loadedAssembly = Assembly.Load(dllBytes);
                if (loadedAssembly != null)
                {
                    _loadedAssemblies[assemblyName] = loadedAssembly;
                    Debug.Log($"[HybridCLRLoader] HotUpdate assembly loaded: {assemblyName}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[HybridCLRLoader] Failed to load HotUpdate assembly {assemblyName}: {e.Message}");
            }
        }

        public static void InitializeGameEntry()
        {
            foreach (var assembly in _loadedAssemblies.Values)
            {
                var gameEntryType = assembly.GetType("HotUpdate.GameEntry");
                if (gameEntryType != null)
                {
                    _gameEntry = Activator.CreateInstance(gameEntryType) as IGameEntry;
                    if (_gameEntry != null)
                    {
                        Debug.Log("[HybridCLRLoader] GameEntry created successfully");
                        _gameEntry.Initialize();
                        return;
                    }
                }
            }
            Debug.LogError("[HybridCLRLoader] GameEntry not found in any loaded assembly");
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
            _loadedAssemblies.Clear();
            _config = null;
        }
    }
}
