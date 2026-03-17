using System;

namespace AOT
{
    public static class AOTBridge
    {
        public static void LogMessage(string message)
        {
            UnityEngine.Debug.Log($"[AOT] {message}");
        }
    }
}
