using UnityEngine;
using UnityEditor;

namespace Editor
{
    public static class EditorTools
    {
        [MenuItem("Tools/Hello Editor")]
        public static void HelloEditor()
        {
            Debug.Log("hello editor");
        }
    }
}
