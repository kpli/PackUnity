using UnityEngine;

namespace AOT
{
    public class GameLauncher : MonoBehaviour
    {
        private static GameLauncher _instance;
        public static GameLauncher Instance => _instance;

        [Header("配置")]
        [SerializeField] private HybridCLRConfig hybridCLRConfig;
        [SerializeField] private bool checkUpdateOnStart = true;

        private enum LaunchState
        {
            None,
            Initializing,
            LoadingAOT,
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
            if (hybridCLRConfig == null)
            {
                Debug.LogError("[GameLauncher] HybridCLRConfig not assigned!");
                return;
            }

            _state = LaunchState.Initializing;
            Debug.Log("[GameLauncher] Step 1: Initializing HybridCLR...");
            HybridCLRLoader.Initialize(hybridCLRConfig);

            _state = LaunchState.LoadingAOT;
            Debug.Log("[GameLauncher] Step 2: Loading AOT assemblies...");
            HybridCLRLoader.LoadAOTAssemblies();

            _state = LaunchState.LoadingHotUpdate;
            Debug.Log("[GameLauncher] Step 3: Loading HotUpdate assemblies...");
            HybridCLRLoader.LoadHotUpdateAssemblies();

            _state = LaunchState.Running;
            Debug.Log("[GameLauncher] Step 4: Initializing GameEntry...");
            HybridCLRLoader.InitializeGameEntry();
        }

        private void Start()
        {
            if (_state == LaunchState.Running)
            {
                HybridCLRLoader.StartGame();
            }
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
                _instance = null;
            }
        }
    }
}
