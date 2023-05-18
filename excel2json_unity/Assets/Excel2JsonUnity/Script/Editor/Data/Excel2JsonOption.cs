using UnityEditor;
using UnityEngine;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// 导出选项
    /// </summary>
    public class Excel2JsonOption
    {
        /// <summary>
        /// 是否导出c#文件
        /// </summary>
        public bool explortCsharp;

        /// <summary>
        /// 单文件导出模式
        /// </summary>
        public string singleExcelPath;

        /// <summary>
        /// 错误代码
        /// </summary>
        public Excel2JsonErrorCode errorCode;

        /// <summary>
        /// 自定义错误消息，用于那些需要填参数的错误信息
        /// </summary>
        public string customErrorMsg;

        /// <summary>
        /// 导出规则
        /// </summary>
        public Excel2JsonRules Rules { get; private set; }

        //重置
        public void Reset()
        {
            errorCode = Excel2JsonErrorCode.None;
            customErrorMsg = string.Empty;
        }

        public void Init()
        {
            Rules = Excel2JsonUtility.GetAsset<Excel2JsonRules>(Excel2JsonConfig.RulesAssetPath);
        }

    }
}