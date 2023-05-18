using UnityEditor;
using UnityEngine;

namespace Excel2JsonUnity.Editor
{
    public static class Excel2JsonUtility
    {
        public static T GetAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            //没有就创建一个资源出来
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}