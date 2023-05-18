using System.IO;
using UnityEditor;
using UnityEngine;

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
        public string excelDirectory = "";

        /// <summary>
        /// 导出的json目录
        /// </summary>
        public string exportJsonDirectory = "";

        /// <summary>
        /// 导出的c#代码目录
        /// </summary>
        public string exportCsharpDirectory = "";

        /// <summary>
        /// 导出的c#代码命名空间
        /// </summary>
        public string csharpNamespace = "Excel2JsonSample";

        /// <summary>
        /// 继承类对象，没有则不写
        /// </summary>
        public string inheritClass = "";

        /// <summary>
        /// 是否压缩json
        /// </summary>
        [Header("是否压缩Json文件")] public bool compressJson = false;
    }

    [CustomEditor(typeof(Excel2JsonRules))]
    public class Excel2JsonRulesEditor : UnityEditor.Editor
    {
        private enum Selection
        {
            ExcelDirectory,
            ExportJsonDirectory,
            ExportCsharpDirectory,
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var rules = (Excel2JsonRules)target;
            GUILayout.Space(20);
            GUILayout.Label("以下为快速设置按钮", GUI.skin.box);
            DrawSelectDirectory(rules, Selection.ExcelDirectory);
            DrawSelectDirectory(rules, Selection.ExportCsharpDirectory);
            DrawSelectDirectory(rules, Selection.ExportJsonDirectory);

            if (GUILayout.Button("Save All"))
            {
                EditorUtility.SetDirty(rules);
                AssetDatabase.SaveAssetIfDirty(rules);
                AssetDatabase.Refresh();
            }
        }

        #region 目录设置

        private void ActionClickSelect(Excel2JsonRules rules, Selection selection)
        {
            var selected = EditorUtility.OpenFolderPanel("Please Select Directory", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
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

        private void ActionClickReveal(Excel2JsonRules rules, Selection selection)
        {
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

        private void DrawSelectDirectory(Excel2JsonRules rules, Selection selection)
        {
            var title = string.Empty;
            switch (selection)
            {
                case Selection.ExcelDirectory:
                    title = "Excel根目录设置";
                    break;
                case Selection.ExportCsharpDirectory:
                    title = "导出c#文件根目录设置";
                    break;
                case Selection.ExportJsonDirectory:
                    title = "导出Json文件根目录设置";
                    break;
            }

            GUILayout.Label(title, GUI.skin.box);
            GUILayout.BeginHorizontal();
            //选择excel目录
            if (GUILayout.Button("Select"))
            {
                ActionClickSelect(rules, selection);
            }

            //前往excel目录
            if (GUILayout.Button("Reveal In Finder"))
            {
                ActionClickReveal(rules, selection);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);
        }

        #endregion
    }
}