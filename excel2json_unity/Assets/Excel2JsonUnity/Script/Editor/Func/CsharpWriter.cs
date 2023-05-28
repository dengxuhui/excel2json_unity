using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// c#文件写入器
    /// </summary>
    public static class CsharpWriter
    {
        //写c#文件
        public static void Start(Excel2JsonOption option, Action<float, string> progressCallBack,
            Dictionary<string, Dictionary<string, string>> csharpDic)
        {
            var curr = 1;
            var total = csharpDic.Count;
            var rules = option.Rules;
            var namespaceStr = rules.csharpNamespace;
            //计算继承类
            Type inheritType = null;
            if (!string.IsNullOrEmpty(rules.runtimeAssembly) && !string.IsNullOrEmpty(rules.inheritClassFullName))
            {
                var ab = Assembly.Load(rules.runtimeAssembly);
                if (ab != null)
                {
                    inheritType = ab.GetType(rules.inheritClassFullName);
                }
            }

            foreach (var kv in csharpDic)
            {
                var xlsxPath = kv.Key;
                var fieldDic = kv.Value;
                var csharpClassName = Path.GetFileNameWithoutExtension(xlsxPath);
                var csharpPath = Path.Combine(rules.exportCsharpDirectory,
                    csharpClassName + ".cs");
                progressCallBack.Invoke((float)curr / total, "正在写入c#文件:" + csharpPath);
                using (var sw = new StreamWriter(csharpPath, false, Encoding.UTF8))
                {
                    //先写入命名空间
                    if (!string.IsNullOrEmpty(namespaceStr))
                    {
                        sw.WriteLine($"namespace {namespaceStr}");
                        sw.WriteLine("{");
                    }

                    //写入注释
                    sw.WriteLine("//Auto Generated Code, Don't Modify");
                    sw.WriteLine("//see https://github.com/dengxuhui/excel2json_unity");
                    sw.WriteLine($"//Generate From {Path.GetFileName(xlsxPath)}");

                    //写class
                    if (inheritType != null)
                    {
                        sw.WriteLine($"public class {csharpClassName} : {rules.inheritClassFullName}");
                    }
                    else
                    {
                        sw.WriteLine($"public class {csharpClassName}");
                    }

                    //class正括号
                    sw.WriteLine("{");

                    //开始写属性
                    foreach (var kv2 in fieldDic)
                    {
                        var fieldName = kv2.Key;
                        var fieldType = kv2.Value;
                        //父类存在的字段
                        if (inheritType != null && inheritType.GetField(fieldName) != null)
                        {
                            continue;
                        }

                        sw.WriteLine($"\tpublic {fieldType} {fieldName};");
                    }

                    //class反括号
                    sw.WriteLine("}");


                    //写命名空间反括号
                    if (!string.IsNullOrEmpty(namespaceStr))
                    {
                        sw.WriteLine("}");
                    }

                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }

                curr++;
            }
        }
    }
}