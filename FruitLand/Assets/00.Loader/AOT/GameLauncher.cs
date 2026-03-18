using UnityEngine;

namespace AOT
{
    public class GameLauncher : MonoBehaviour
    {
        private static GameLauncher _instance;
        public static GameLauncher Instance => _instance;

        [Header("YooAsset Settings")]
        [SerializeField] private string packageName = "DefaultPackage";
        [SerializeField] private bool checkUpdateOnStart = true;

        private enum LaunchState
        {
            None,
            InitializingYooAsset,
            CheckingUpdate,
            DownloadingUpdate,
            LoadingHotUpdate,
            Running
        }

        private LaunchState _state = LaunchState.None;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            StartLaunchSequence();
        }

        private void StartLaunchSequence()
        {
            _state = LaunchState.InitializingYooAsset;
            Debug.Log("[GameLauncher] Step 1: Initializing YooAsset...");

            YooAssetManager.Instance.Initialize(packageName, OnYooAssetInitialized, OnYooAssetInitFailed);
        }

        private void OnYooAssetInitialized()
        {
            Debug.Log("[GameLauncher] YooAsset initialized successfully");

            if (checkUpdateOnStart)
            {
                _state = LaunchState.CheckingUpdate;
                Debug.Log("[GameLauncher] Step 2: Checking for updates...");
                YooAssetManager.Instance.UpdatePackage(packageName, OnUpdateComplete, OnUpdateFailed, OnUpdateProgress);
            }
            else
            {
                LoadHotUpdateAssembly();
            }
        }

        private void OnYooAssetInitFailed(string error)
        {
            Debug.LogError($"[GameLauncher] YooAsset initialization failed: {error}");
            LoadHotUpdateAssembly();
        }

        private void OnUpdateComplete()
        {
            Debug.Log("[GameLauncher] Package update completed");
            LoadHotUpdateAssembly();
        }

        private void OnUpdateFailed(string error)
        {
            Debug.LogError($"[GameLauncher] Package update failed: {error}");
            LoadHotUpdateAssembly();
        }

        private void OnUpdateProgress(float progress)
        {
            Debug.Log($"[GameLauncher] Download progress: {progress:P0}");
        }

        private void LoadHotUpdateAssembly()
        {
            _state = LaunchState.LoadingHotUpdate;
            Debug.Log("[GameLauncher] Step 3: Loading HotUpdate assembly...");
            HybridCLRLoader.LoadHotUpdate();
            _state = LaunchState.Running;

            HybridCLRLoader.StartGame();
        }

        private void Update()
        {
            if (_state == LaunchState.Running)
            {
                HybridCLRLoader.Update(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                HybridCLRLoader.Shutdown();
                YooAssetManager.Instance.Cleanup();
                _instance = null;
            }
        }
    }
}
