using System;
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

            var operation = _package.InitializeAsync(new EditorSimulateModeParameters());
            operation.Completed += (op) =>
            {
                if (op.Status == EOperationStatus.Succeed)
                {
                    _isInitialized = true;
                    Debug.Log($"[YooAssetManager] Package '{packageName}' initialized successfully");
                    onSuccess?.Invoke();
                }
                else
                {
                    Debug.LogError($"[YooAssetManager] Failed to initialize package: {op.Error}");
                    onFailed?.Invoke(op.Error);
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

            var operation = _package.RequestPackageVersionAsync();
            operation.Completed += (op) =>
            {
                if (op.Status != EOperationStatus.Succeed)
                {
                    onFailed?.Invoke($"Request version failed: {op.Error}");
                    return;
                }

                var versionOp = op as RequestPackageVersionOperation;
                var updateOperation = _package.UpdatePackageManifestAsync(versionOp.PackageVersion);
                updateOperation.Completed += (updateOp) =>
                {
                    if (updateOp.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log($"[YooAssetManager] Package updated to version {versionOp.PackageVersion}");
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        onFailed?.Invoke($"Update manifest failed: {updateOp.Error}");
                    }
                };
            };
        }

        public T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object
        {
            if (!_isInitialized)
            {
                Debug.LogError("[YooAssetManager] Not initialized");
                return null;
            }

            var handle = _package.LoadAssetSync<T>(assetPath);
            return handle.AssetObject as T;
        }

        public void LoadAssetAsync<T>(string assetPath, Action<T> onComplete, Action<string> onFailed = null) where T : UnityEngine.Object
        {
            if (!_isInitialized)
            {
                onFailed?.Invoke("YooAsset not initialized");
                return;
            }

            var handle = _package.LoadAssetAsync<T>(assetPath);
            handle.Completed += (h) =>
            {
                if (h.Status == EOperationStatus.Succeed)
                {
                    onComplete?.Invoke(h.AssetObject as T);
                }
                else
                {
                    onFailed?.Invoke(h.LastError);
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
                _package.ClearCacheFilesAsync(EFileClearMode.ClearAllBundleFiles);
            }
            _isInitialized = false;
            Debug.Log("[YooAssetManager] Cleanup completed");
        }
    }
}
