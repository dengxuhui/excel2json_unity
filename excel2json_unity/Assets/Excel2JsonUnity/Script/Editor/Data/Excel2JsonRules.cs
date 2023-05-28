using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// 配置规则
    /// </summary>
    public class Excel2JsonRules : ScriptableObject
    {
        /// <summary>
        /// excel文件根目录
        /// </summary>
        [Tooltip("目录需要满足在Unity项目根目录下")] public string excelDirectory = "";

        /// <summary>
        /// 导出的json目录
        /// </summary>
        [Tooltip("目录需要满足在Unity项目根目录下")] public string exportJsonDirectory = "";

        /// <summary>
        /// 导出的c#代码目录
        /// </summary>
        [Tooltip("目录需要满足在Unity项目根目录下")] public string exportCsharpDirectory = "";

        /// <summary>
        /// 导出的c#代码命名空间
        /// </summary>
        [Tooltip("空字符串表示没有Namespace，请按需设置")] public string csharpNamespace = "";

        /// <summary>
        /// 运行时assembly
        /// </summary>
        [Tooltip("运行时程序集")] 
        public string runtimeAssembly = "Assembly-CSharp";
        /// <summary>
        /// 继承类对象，没有则不写
        /// </summary>
        [Tooltip("需要assembly-qualified类型全名字段\nex:SampleNamespace.SampleClass")]
        public string inheritClassFullName = "";
        
        /// <summary>
        /// 是否压缩json
        /// </summary>
        [Tooltip("勾选之后导出的Json文件会去除换行符与缩进符")] public bool compressJson = false;

        //默认值
        public int defaultInt = 0;
        public string defaultString = "";
        public float defaultFloat = 0.0f;
        [Tooltip("数组的默认值，设置中按照字符串表达，空数组配置[]，也可以配置null")]
        public string defaultArray = "[]";

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(Excel2JsonUtility.GetAsset<Excel2JsonRules>(Excel2JsonConfig.RulesAssetPath));
        }

        internal static Excel2JsonRules GetAsset()
        {
            return Excel2JsonUtility.GetAsset<Excel2JsonRules>(Excel2JsonConfig.RulesAssetPath);
        }
    }

    static class Excel2JsonRulesSettingsRegister
    {
        private enum Selection
        {
            ExcelDirectory,
            ExportJsonDirectory,
            ExportCsharpDirectory,
        }

        #region 点击响应

        private static void ActionClickSelect(Selection selection)
        {
            var rules = Excel2JsonRules.GetAsset();
            var selected = EditorUtility.OpenFolderPanel($"Please Select Directory::{selection.ToString()}",
                Application.dataPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                selected = selected.Replace(Application.dataPath.Replace("Assets", ""), "");
                switch (selection)
                {
                    case Selection.ExcelDirectory:
                        rules.excelDirectory = selected;
                        break;
                    case Selection.ExportCsharpDirectory:
                        rules.exportCsharpDirectory = selected;
                        break;
                    case Selection.ExportJsonDirectory:
                        rules.exportJsonDirectory = selected;
                        break;
                }

                EditorUtility.SetDirty(rules);
                AssetDatabase.SaveAssetIfDirty(rules);
            }
        }

        private static void ActionClickReveal(Selection selection)
        {
            var rules = Excel2JsonRules.GetAsset();
            var dir = string.Empty;
            switch (selection)
            {
                case Selection.ExcelDirectory:
                    dir = rules.excelDirectory;
                    break;
                case Selection.ExportCsharpDirectory:
                    dir = rules.exportCsharpDirectory;
                    break;
                case Selection.ExportJsonDirectory:
                    dir = rules.exportJsonDirectory;
                    break;
            }

            if (Directory.Exists(dir))
            {
                EditorUtility.RevealInFinder(dir);
            }
            else
            {
                EditorUtility.DisplayDialog("Reveal in Finder", "error:dir is not exist", "OK");
            }
        }

        #endregion

        [SettingsProvider]
        public static SettingsProvider CreateRulesSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Excel2JsonSettings", SettingsScope.Project)
            {
                label = "Excel2Json",
                guiHandler = (searchContext) =>
                {
                    var subtitleStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontStyle = FontStyle.BoldAndItalic,
                        fontSize = 16,
                        stretchHeight = true,
                    };
                    var settings = Excel2JsonRules.GetSerializedSettings();
                    //C#导出设置
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("C# export settings", subtitleStyle);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(settings.FindProperty("exportCsharpDirectory"),
                        new GUIContent("Export C# Directory"));
                    if (GUILayout.Button("Select"))
                    {
                        ActionClickSelect(Selection.ExportCsharpDirectory);
                    }

                    if (GUILayout.Button("Reveal In Finder"))
                    {
                        ActionClickReveal(Selection.ExportCsharpDirectory);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(settings.FindProperty("csharpNamespace"),
                        new GUIContent("C# Namespace"));
                    EditorGUILayout.PropertyField(settings.FindProperty("runtimeAssembly"),
                        new GUIContent("Run Time Assembly"));
                    EditorGUILayout.PropertyField(settings.FindProperty("inheritClassFullName"),
                        new GUIContent("Inherit Class FullName"));
                    //Json导出设置
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Json export settings", subtitleStyle);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(settings.FindProperty("exportJsonDirectory"),
                        new GUIContent("Export Json Directory"));
                    if (GUILayout.Button("Select"))
                    {
                        ActionClickSelect(Selection.ExportJsonDirectory);
                    }

                    if (GUILayout.Button("Reveal In Finder"))
                    {
                        ActionClickReveal(Selection.ExportJsonDirectory);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(settings.FindProperty("compressJson"),
                        new GUIContent("Compress Json"));
                    //excel相关设置
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Excel settings", subtitleStyle);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(settings.FindProperty("excelDirectory"),
                        new GUIContent("Excel Root Directory"));
                    if (GUILayout.Button("Select"))
                    {
                        ActionClickSelect(Selection.ExcelDirectory);
                    }

                    if (GUILayout.Button("Reveal In Finder"))
                    {
                        ActionClickReveal(Selection.ExcelDirectory);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Field Default Value Settings", subtitleStyle);
                    EditorGUILayout.PropertyField(settings.FindProperty("defaultInt"), new GUIContent("Int"));
                    EditorGUILayout.PropertyField(settings.FindProperty("defaultFloat"), new GUIContent("Float"));
                    EditorGUILayout.PropertyField(settings.FindProperty("defaultString"), new GUIContent("String"));
                    EditorGUILayout.PropertyField(settings.FindProperty("defaultArray"), new GUIContent("Array"));

                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                keywords = new HashSet<string>(new[] { "Export", "Excel", "C#", "Json" })
            };

            return provider;
        }
    }
}