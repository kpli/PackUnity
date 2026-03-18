using System;

namespace Core
{
    public static class Log
    {
        public static void Info(string message)
        {
            UnityEngine.Debug.Log($"[Core] {message}");
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning($"[Core] {message}");
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError($"[Core] {message}");
        }
    }
}
