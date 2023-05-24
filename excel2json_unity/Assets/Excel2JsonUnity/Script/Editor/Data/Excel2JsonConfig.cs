using System.Collections.Generic;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// 导出相关配置
    /// </summary>
    public static class Excel2JsonConfig
    {
        //通用标题
        public static readonly string Title = "Excel2JsonUtility";

        //表头数量
        public static readonly int RowHeaderCount = 3;

        //规则资源路径
        public static readonly string RulesAssetPath = "Assets/Excel2JsonUnity/Excel2JsonRules.asset";


        //类型映射表
        public static readonly Dictionary<string, string> DataTypes = new Dictionary<string, string>
        {
            { "string", "string" },
            { "int", "int" },
            { "float", "float" },
            { "string[]", "System.Collections.Generic.List<string>" },
            { "int[]", "System.Collections.Generic.List<int>" },
            { "float[]", "System.Collections.Generic.List<float>" },
            //下面是自定义数据类型
            { "SampleCustomData", "Excel2JsonUnity.Sample.SampleCustomData" },
        };

        #region 错误消息定义

        //错误消息定义
        public static readonly Dictionary<Excel2JsonErrorCode, string> ErrorMsg =
            new Dictionary<Excel2JsonErrorCode, string>
            {
                { Excel2JsonErrorCode.ExcelDirectoryNotSet, "excel目录未设定" },
                { Excel2JsonErrorCode.JsonDirectoryNotSet, "json导出目录未设定" },
                { Excel2JsonErrorCode.CSharpDirectoryNotSet, "c#导出目录未设定" },
                { Excel2JsonErrorCode.AnyExcelFileNotFound, "没找到任意excel文件" },
                { Excel2JsonErrorCode.ExcelFileReadFail, "excel文件读取失败：{0}" },
                { Excel2JsonErrorCode.FieldSettingError, "字段设置错误：{0}" },
                { Excel2JsonErrorCode.TypeNotDefined, "类型未定义：{0}" },
                { Excel2JsonErrorCode.FieldValueIsNotSpecifiedType, "字段值类型错误：{0}" },
                { Excel2JsonErrorCode.UnknownFieldType, "未知数据类型：{0}" },
            };

        //未知错误
        public static readonly string UnknownErrorMsg = "未知错误？";

        #endregion
    }

    /// <summary>
    /// 导出错误代码
    /// </summary>
    public enum Excel2JsonErrorCode
    {
        //无错误
        None,

        //excel目录未设定
        ExcelDirectoryNotSet,

        //json导出目录未设定
        JsonDirectoryNotSet,

        //c#导出目录未设定
        CSharpDirectoryNotSet,

        //没找到excel文件
        AnyExcelFileNotFound,

        //excel文件读取失败
        ExcelFileReadFail,

        //字段设置错误
        FieldSettingError,

        //类型未定义
        TypeNotDefined,

        //字段值类型错误
        FieldValueIsNotSpecifiedType,

        //未知数据类型
        UnknownFieldType,
    }
}