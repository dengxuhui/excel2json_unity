using System.IO;

namespace Excel2JsonUnity.Editor
{
    public static class Checker
    {
        /// <summary>
        /// 执行导出前的检查
        /// </summary>
        public static void DoStartCheck(Excel2JsonOption option)
        {
            var rule = option.Rules;
            //不是单文件导出，需要检查excel目录
            if (string.IsNullOrEmpty(option.singleExcelPath) &&
                (string.IsNullOrEmpty(rule.excelDirectory) || !Directory.Exists(rule.excelDirectory)))
            {
                option.errorCode = Excel2JsonErrorCode.ExcelDirectoryNotSet;
                return;
            }

            if (string.IsNullOrEmpty(rule.exportJsonDirectory) || !Directory.Exists(rule.exportJsonDirectory))
            {
                option.errorCode = Excel2JsonErrorCode.JsonDirectoryNotSet;
                return;
            }

            if (option.explortCsharp && (string.IsNullOrEmpty(rule.exportCsharpDirectory) ||
                                         !Directory.Exists(rule.exportCsharpDirectory)))
            {
                option.errorCode = Excel2JsonErrorCode.CSharpDirectoryNotSet;
                return;
            }
        }

        /// <summary>
        /// 完成检查
        /// </summary>
        /// <param name="option"></param>
        public static void DoStopCheck(Excel2JsonOption option)
        {
        }
    }
}