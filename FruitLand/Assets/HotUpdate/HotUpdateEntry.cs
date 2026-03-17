using System;
using AOT;

namespace HotUpdate
{
    public class HotUpdateEntry
    {
        public static void Initialize()
        {
            AOTBridge.LogMessage("HotUpdate initialized!");
            UnityEngine.Debug.Log("[HotUpdate] Entry point executed");
        }
    }
}
