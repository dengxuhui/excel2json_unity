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

        /// <summary>
        /// 创建错误消息
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="appendMsg"></param>
        /// <returns></returns>
        public static string CreateCustomErrorMsg(Excel2JsonErrorCode errorCode, string appendMsg = "")
        {
            if (Excel2JsonConfig.ErrorMsg.TryGetValue(errorCode, out var msg))
            {
                return string.Format(msg, appendMsg);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}