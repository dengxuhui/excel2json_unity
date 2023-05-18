using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// json数据写入器
    /// </summary>
    public static class JsonWriter
    {
        //写json数据
        public static void Start(Excel2JsonOption option, Action<float, string> progressCallBack,
            Dictionary<string, string> jsonDic)
        {
            var curr = 1;
            var total = jsonDic.Count;
            if (jsonDic.Count <= 0)
            {
                return;
            }

            var rules = option.Rules;
            foreach (var kv in jsonDic)
            {
                var xlsxPath = kv.Key;
                var jsonPath = Path.Combine(rules.exportJsonDirectory,
                    Path.GetFileNameWithoutExtension(xlsxPath) + ".json");
                var jsonStr = kv.Value;
                progressCallBack.Invoke((float)curr / total, "正在写入json数据:" + jsonPath);
                if (rules.compressJson)
                {
                    jsonStr = jsonStr.Replace("\t", "");
                    jsonStr = jsonStr.Replace("\n", "");
                }

                using var sw = new StreamWriter(jsonPath, false, Encoding.UTF8);
                sw.Write(jsonStr);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                curr++;
            }
        }
    }
}