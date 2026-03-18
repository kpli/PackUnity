using System;
using System.Collections;
using UnityEngine;
using YooAsset;

namespace AOT
{
    public class YooAssetManager : IAssetManager
    {
        private static YooAssetManager _instance;
        public static YooAssetManager Instance => _instance ??= new YooAssetManager();

        private ResourcePackage _package;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        private YooAssetManager() { }

        public void Initialize(string packageName, Action onSuccess, Action<string> onFailed)
        {
            if (_isInitialized)
            {
                onSuccess?.Invoke();
                return;
            }

            YooAssets.Initialize();
            _package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(_package);

            var operation = _package.InitializePackage(new EditorSimulateModeParameters());
            operation.OnComplete += () =>
            {
                if (operation.Status == EOperationStatus.Succeed)
                {
                    _isInitialized = true;
                    Debug.Log($"[YooAssetManager] Package '{packageName}' initialized successfully");
                    onSuccess?.Invoke();
                }
                else
                {
                    Debug.LogError($"[YooAssetManager] Failed to initialize package: {operation.Error}");
                    onFailed?.Invoke(operation.Error);
                }
            };
        }

        public void UpdatePackage(string packageName, Action onSuccess, Action<string> onFailed, Action<float> onProgress)
        {
            if (!_isInitialized)
            {
                onFailed?.Invoke("YooAsset not initialized");
                return;
            }

            var operation = _package.UpdatePackageVersionAsync();
            operation.OnComplete += () =>
            {
                if (operation.Status != EOperationStatus.Succeed)
                {
                    onFailed?.Invoke($"Update version failed: {operation.Error}");
                    return;
                }

                var updateOperation = _package.UpdatePackageManifestAsync(operation.PackageVersion);
                updateOperation.OnProgress += progress => onProgress?.Invoke(progress);
                updateOperation.OnComplete += () =>
                {
                    if (updateOperation.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log($"[YooAssetManager] Package updated to version {operation.PackageVersion}");
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        onFailed?.Invoke($"Update manifest failed: {updateOperation.Error}");
                    }
                };
            };
        }

        public T LoadAssetSync<T>(string assetPath) where T : Object
        {
            if (!_isInitialized)
            {
                Debug.LogError("[YooAssetManager] Not initialized");
                return null;
            }

            var handle = _package.LoadAssetSync<T>(assetPath);
            return handle.AssetObject as T;
        }

        public void LoadAssetAsync<T>(string assetPath, Action<T> onComplete, Action<string> onFailed = null) where T : Object
        {
            if (!_isInitialized)
            {
                onFailed?.Invoke("YooAsset not initialized");
                return;
            }

            var handle = _package.LoadAssetAsync<T>(assetPath);
            handle.OnComplete += () =>
            {
                if (handle.Status == EOperationStatus.Succeed)
                {
                    onComplete?.Invoke(handle.AssetObject as T);
                }
                else
                {
                    onFailed?.Invoke(handle.Error);
                }
            };
        }

        public void UnloadAsset(string assetPath)
        {
            Debug.Log($"[YooAssetManager] UnloadAsset called for: {assetPath}");
        }

        public void Cleanup()
        {
            if (_package != null)
            {
                _package.ClearPackageCache();
            }
            _isInitialized = false;
            Debug.Log("[YooAssetManager] Cleanup completed");
        }
    }
}
