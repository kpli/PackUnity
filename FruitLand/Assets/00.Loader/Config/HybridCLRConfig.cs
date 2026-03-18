using System.Collections.Generic;
using UnityEngine;

namespace AOT
{
    [CreateAssetMenu(fileName = "HybridCLRConfig", menuName = "AOT/HybridCLR Config")]
    public class HybridCLRConfig : ScriptableObject
    {
        [Header("AOT 程序集配置")]
        [Tooltip("AOT 程序集列表，这些 DLL 不会被热更新")]
        public List<string> aotAssemblies = new List<string>
        {
            "Core",
            "AOT"
        };

        [Header("热更新程序集配置")]
        [Tooltip("热更新程序集列表，这些 DLL 可以从服务器下载更新")]
        public List<string> hotUpdateAssemblies = new List<string>
        {
            "HotUpdate"
        };

        [Header("DLL 加载路径")]
        [Tooltip("热更新 DLL 的加载路径（相对于 StreamingAssets）")]
        public string hotUpdateDllPath = "HotUpdateDlls";

        [Tooltip("AOT 补充元数据 DLL 路径")]
        public string aotMetadataPath = "AOTMetadata";

        [Header("启动配置")]
        [Tooltip("启动时自动加载热更新 DLL")]
        public bool autoLoadHotUpdate = true;

        [Tooltip("启动时检查 AOT 元数据补丁")]
        public bool patchAOTMetadata = true;
    }
}
