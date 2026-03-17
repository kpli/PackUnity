using UnityEngine;

namespace AOT
{
    public class GameLauncher : MonoBehaviour
    {
        private static GameLauncher _instance;

        public static GameLauncher Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("[GameLauncher] Loading HotUpdate assembly...");
            HybridCLRLoader.LoadHotUpdate();
        }

        private void Start()
        {
            HybridCLRLoader.StartGame();
        }

        private void Update()
        {
            HybridCLRLoader.Update(Time.deltaTime);
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
