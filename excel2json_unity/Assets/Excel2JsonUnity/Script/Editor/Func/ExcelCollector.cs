using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Excel;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// excel文件收集器
    /// </summary>
    public static class ExcelCollector
    {
        /// <summary>
        /// 执行excel数据收集
        /// </summary>
        /// <param name="option">导出参数</param>
        /// <param name="progressCallBack">进度回调函数</param>
        /// <param name="jsonStrMap">收集到的json字符串，key：Excel文件路径，value：导出的字符串（用来直接写文件）</param>
        /// <param name="csharpTypeMap">从配置到c#对象的映射，key：Excel文件路径，value：一个文件中的c#字段定义（key：json字段名，value：c#数据类型</param>
        public static void Start(Excel2JsonOption option, Action<float, string> progressCallBack,
            out Dictionary<string, string> jsonStrMap,
            out Dictionary<string, Dictionary<string, string>> csharpTypeMap)
        {
            jsonStrMap = new Dictionary<string, string>();
            csharpTypeMap = new Dictionary<string, Dictionary<string, string>>();
            var rules = option.Rules;
            var excelFiles = !string.IsNullOrEmpty(option.singleExcelPath)
                ? new string[1] { option.singleExcelPath }
                : Directory.GetFiles(rules.excelDirectory, "*.xlsx", SearchOption.AllDirectories);
            if (excelFiles.Length <= 0)
            {
                option.errorCode = Excel2JsonErrorCode.AnyExcelFileNotFound;
                return;
            }

            for (var i = 0; i < excelFiles.Length; i++)
            {
                var filePath = excelFiles[i];
                progressCallBack.Invoke(i + 1 / excelFiles.Length, $"读取配置档文件：{filePath}");
                var result = CollectXlsx(option, filePath, out var jsonStr, out var csharpType);
                if (option.errorCode != Excel2JsonErrorCode.None)
                {
                    return;
                }

                if (!result) continue;
                jsonStrMap.Add(filePath, jsonStr);
                csharpTypeMap.Add(filePath, csharpType);
            }
        }

        #region private

        //收集单个文件
        private static bool CollectXlsx(Excel2JsonOption option, string filePath, out string jsonStr,
            out Dictionary<string, string> csharpTypeMap)
        {
            csharpTypeMap = new Dictionary<string, string>();
            jsonStr = string.Empty;
            option.collectingExcelPath = filePath;
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            if (!string.IsNullOrEmpty(reader.ExceptionMessage))
            {
                option.errorCode = Excel2JsonErrorCode.ExcelFileReadFail;
                option.customErrorMsg =
                    Excel2JsonUtility.CreateCustomErrorMsg(option.errorCode, reader.ExceptionMessage);

                return false;
            }

            //todo 潜在规则，需要在README中说明，一个Excel文件中只存放一张表，并且Sheet名以#开头和结尾
            var result = reader.AsDataSet();
            var count = result.Tables.Count;
            if (count <= 0)
            {
                return false;
            }

            var fileName = Path.GetFileName(filePath);
            var table = result.Tables[0];
            return CollectTable(option, fileName, table, ref jsonStr, ref csharpTypeMap);
        }

        private static bool CollectTable(Excel2JsonOption option, string fileName, DataTable table, ref string jsonStr,
            ref Dictionary<string, string> csharpTypeMap)
        {
            if (!table.TableName.StartsWith("#") || !table.TableName.EndsWith("#"))
            {
                return false;
            }

            var rowCount = table.Rows.Count;
            //空表，只有定义
            if (rowCount <= Excel2JsonConfig.RowHeaderCount)
            {
                return false;
            }

            var sb = new StringBuilder();
            sb.Append("[\n");

            //列数据
            var columns = table.Columns;
            var colCount = columns.Count;
            //行数据
            var rowDataMat = table.Rows;
            //可以只记录组类型
            var processedList = new List<int>();
            var tempStrList = new List<string>();
            //按行读取数据
            for (int i = Excel2JsonConfig.RowHeaderCount; i < rowCount; i++)
            {
                processedList.Clear();
                tempStrList.Clear();
                sb.Append("\t{\n");
                //读取每列数据
                for (int j = 0; j < colCount; j++)
                {
                    var fieldName = rowDataMat[0][j].ToString();
                    var fieldType = rowDataMat[1][j].ToString();
                    //没有设置类型与名字就认定为注释列
                    if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(fieldType))
                    {
                        processedList.Add(j);
                        continue;
                    }

                    //如果包含点，说明是组合字段
                    var isFieldNameGroup = fieldName.Contains(".");
                    var isFieldTypeGroup = fieldType.Contains(".");
                    if (isFieldNameGroup && isFieldTypeGroup)
                    {
                        //此列已经处理了就不再处理
                        if (processedList.IndexOf(j) > 0)
                        {
                            continue;
                        }

                        fieldName = fieldName.Split(".")[0];
                        var cellStr = HandleCombineCell(option, i, colCount, rowDataMat, fieldName, ref processedList);
                        tempStrList.Add(cellStr);
                    }
                    else if (isFieldNameGroup)
                    {
                        //字段名是组合，类型不是组合，报错
                        option.errorCode = Excel2JsonErrorCode.FieldSettingError;
                        option.customErrorMsg =
                            Excel2JsonUtility.CreateCustomErrorMsg(option.errorCode, $"文件：{fileName} 字段：{fieldName}");
                        return false;
                    }
                    else
                    {
                        //普通字段
                        var cellStr = HandleSingleCell(option, i, j, rowDataMat, ref processedList);
                        tempStrList.Add(cellStr);
                    }
                }

                for (var i1 = 0; i1 < tempStrList.Count; i1++)
                {
                    var str = tempStrList[i1];
                    sb.Append(str);
                    //最后一行不需要逗号
                    sb.Append(i1 != tempStrList.Count - 1 ? ",\n" : "\n");
                }

                sb.Append(i == (rowCount - 1) ? "\t}\n" : "\t},\n");
            }

            sb.Append("]");
            jsonStr = sb.ToString();

            //写入C#类型
            if (option.explortCsharp)
            {
                for (int i = 0; i < colCount; i++)
                {
                    var fieldName = rowDataMat[0][i].ToString();
                    var fieldType = rowDataMat[1][i].ToString();
                    if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(fieldType))
                    {
                        continue;
                    }

                    var isGroupField = fieldName.Contains(".");
                    if (isGroupField)
                    {
                        fieldName = fieldName.Split(".")[0];
                    }

                    if (csharpTypeMap.ContainsKey(fieldName))
                    {
                        continue;
                    }

                    if (isGroupField)
                    {
                        fieldType = fieldType.Split(".")[0];
                    }

                    if (Excel2JsonConfig.DataTypes.TryGetValue(fieldType, out var csharpType))
                    {
                        csharpTypeMap.Add(fieldName, csharpType);
                    }
                    else
                    {
                        option.errorCode = Excel2JsonErrorCode.TypeNotDefined;
                        option.customErrorMsg = Excel2JsonUtility.CreateCustomErrorMsg(option.errorCode,
                            $"文件：{fileName} 字段：{fieldName} 类型：{fieldType}");

                        return false;
                    }
                }
            }

            return true;
        }

        private static string HandleSingleCell(Excel2JsonOption option, int row, int col, DataRowCollection rowDataMat,
            ref List<int> processedList)
        {
            processedList.Add(col);
            var fieldName = rowDataMat[0][col].ToString();
            var fieldType = rowDataMat[1][col].ToString();
            var fieldValue = rowDataMat[row][col];
            var combine = Combine(option, fieldName, fieldType, fieldValue.ToString());
            var result = $"\t\t{combine}";
            return result;
        }

        private static string HandleCombineCell(Excel2JsonOption option, int row, int colCount,
            DataRowCollection rowDataMat,
            string targetFieldName,
            ref List<int> processedList)
        {
            var sb = new StringBuilder();
            sb.Append("\t\t");
            sb.Append($"\"{targetFieldName}\":");
            sb.Append("{");
            var tempStrList = new List<string>();
            for (int i = 0; i < colCount; i++)
            {
                var filedName = rowDataMat[0][i].ToString();
                if (!filedName.StartsWith(targetFieldName))
                {
                    continue;
                }

                var filedType = rowDataMat[1][i].ToString();
                filedType = filedType.Split(".")[1];
                filedName = filedName.Split(".")[1];
                var fieldValue = rowDataMat[row][i];
                var combine = Combine(option, filedName, filedType, fieldValue.ToString());
                tempStrList.Add(combine);
                processedList.Add(i);
            }

            for (var i = 0; i < tempStrList.Count; i++)
            {
                var str = tempStrList[i];
                sb.Append(str);
                if (i != tempStrList.Count - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("}");
            return sb.ToString();
        }

        private static string Combine(Excel2JsonOption option, string fieldName, string fieldType, string fieldValue)
        {
            if (option.errorCode != Excel2JsonErrorCode.None)
            {
                return string.Empty;
            }

            string result = string.Empty;
            switch (fieldType)
            {
                case "int":
                    var intValue = option.Rules.defaultInt;
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        if (Int32.TryParse(fieldValue, out var parse))
                        {
                            intValue = parse;
                        }
                        else
                        {
                            //这里出错无立马返回
                            option.errorCode = Excel2JsonErrorCode.FieldValueIsNotSpecifiedType;
                        }
                    }

                    result = $"\"{fieldName}\":{intValue}";
                    break;
                case "float":
                    var floatValue = option.Rules.defaultFloat;
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        if (float.TryParse(fieldValue, out var parse))
                        {
                            floatValue = parse;
                        }
                        else
                        {
                            option.errorCode = Excel2JsonErrorCode.FieldValueIsNotSpecifiedType;
                        }
                    }

                    result = $"\"{fieldName}\":{floatValue}";
                    break;
                case "string":
                    var stringValue = option.Rules.defaultString;
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        stringValue = fieldValue;
                    }

                    result = $"\"{fieldName}\":\"{stringValue}\"";
                    break;
                case "int[]":
                    result = string.IsNullOrEmpty(fieldValue)
                        ? $"\"{fieldName}\":{option.Rules.defaultArray}"
                        : $"\"{fieldName}\":[{fieldValue}]";
                    break;
                case "float[]":
                    result = string.IsNullOrEmpty(fieldValue)
                        ? $"\"{fieldName}\":{option.Rules.defaultArray}"
                        : $"\"{fieldName}\":[{fieldValue}]";
                    break;
                case "string[]":
                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        result = $"\"{fieldName}\":{option.Rules.defaultArray}";
                    }
                    else
                    {
                        var stringValueArray = fieldValue.Split(",");
                        if (stringValueArray is not { Length: > 0 })
                        {
                            result = $"\"{fieldName}\":{option.Rules.defaultArray}";
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.Append($"\"{fieldName}\":[");
                            for (var i = 0; i < stringValueArray.Length; i++)
                            {
                                sb.Append(i != stringValueArray.Length - 1
                                    ? $"\"{stringValueArray[i]}\","
                                    : $"\"{stringValueArray[i]}\"");
                            }

                            sb.Append("]");
                            result = sb.ToString();
                        }
                    }

                    break;
                default:
                    if (Excel2JsonConfig.DataTypes.TryGetValue(fieldType, out var strType))
                    {
                        var type = option.assembly.GetType(strType);
                        //枚举类型
                        if (type is { IsEnum: true } && Enum.TryParse(type, fieldValue, true, out var toEnum))
                        {
                            result = $"\"{fieldName}\":\"{fieldValue}\"";
                        }
                        else
                        {
                            option.errorCode = Excel2JsonErrorCode.EnumFieldUndefined;
                        }
                    }

                    //尝试解析自定义类型
                    break;
            }


            if (string.IsNullOrEmpty(result) && option.errorCode == Excel2JsonErrorCode.None)
            {
                option.errorCode = Excel2JsonErrorCode.UnknownFieldType;
            }

            if (option.errorCode != Excel2JsonErrorCode.None)
            {
                option.customErrorMsg = Excel2JsonUtility.CreateCustomErrorMsg(option.errorCode,
                    $"#File:{option.collectingExcelPath} #Field:{fieldName} #Value:{fieldValue}");
            }

            return result;
        }

        #endregion
    }
}