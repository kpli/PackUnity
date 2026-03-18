using System;

namespace AOT
{
    public interface IAssetManager
    {
        void Initialize(string packageName, Action onSuccess, Action<string> onFailed);
        void UpdatePackage(string packageName, Action onSuccess, Action<string> onFailed, Action<float> onProgress);
        T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object;
        void LoadAssetAsync<T>(string assetPath, Action<T> onComplete, Action<string> onFailed = null) where T : UnityEngine.Object;
        void UnloadAsset(string assetPath);
        void Cleanup();
    }
}
