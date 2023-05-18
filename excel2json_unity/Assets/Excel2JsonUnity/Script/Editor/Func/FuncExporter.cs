using UnityEditor;
using UnityEngine;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// 导出工具
    /// </summary>
    public static class FuncExporter
    {
        #region private methods

        //完成
        private static void Stop(Excel2JsonOption option)
        {
            Checker.DoStopCheck(option);
            EditorUtility.ClearProgressBar();
            if (option.errorCode <= 0)
            {
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog(Excel2JsonConfig.Title, "一键导出完成", "OK");
            }
            else
            {
                string errorMsg;
                if (string.IsNullOrEmpty(option.customErrorMsg))
                {
                    Excel2JsonConfig.ErrorMsg.TryGetValue(option.errorCode, out errorMsg);
                }
                else
                {
                    errorMsg = option.customErrorMsg;
                }

                if (string.IsNullOrEmpty(errorMsg))
                {
                    errorMsg = Excel2JsonConfig.UnknownErrorMsg;
                }

                var showMsg = $"error: code [{option.errorCode}] msg:{errorMsg}";
                Debug.LogError(showMsg);
                EditorUtility.DisplayDialog(Excel2JsonConfig.Title, showMsg,
                    "OK");
            }
        }

        //尝试终止导出流程
        private static bool TryStopIfError(Excel2JsonOption option)
        {
            if (option.errorCode == Excel2JsonErrorCode.None) return false;
            Stop(option);
            return true;
        }

        //显示进度
        private static void Progress(float progress, string msg = "")
        {
            EditorUtility.DisplayProgressBar(Excel2JsonConfig.Title, msg, progress);
        }

        #endregion


        //开始执行配置导出流程
        public static void Start(Excel2JsonOption option)
        {
            Progress(0.1f, "检查资源");
            Checker.DoStartCheck(option);
            if (TryStopIfError(option)) return;
            ExcelCollector.Start(option, Progress, out var jsonStrMap, out var csharpTypeMap);
            if (TryStopIfError(option)) return;
            JsonWriter.Start(option, Progress, jsonStrMap);
            if (TryStopIfError(option)) return;
            if (option.explortCsharp)
            {
                CsharpWriter.Start(option, Progress, csharpTypeMap);
            }

            Stop(option);
        }
    }
}