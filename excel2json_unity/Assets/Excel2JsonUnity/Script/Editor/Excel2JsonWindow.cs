using System;
using UnityEditor;
using UnityEngine;

namespace Excel2JsonUnity.Editor
{
    /// <summary>
    /// 配置导出window
    /// </summary>
    public sealed class Excel2JsonWindow : EditorWindow
    {
        /// <summary>
        /// 打开窗口
        /// 根据自己项目的需求，自行修改
        /// </summary>
        [MenuItem("Window/Excel2Json")]
        static void Init()
        {
            GetWindow<Excel2JsonWindow>("Excel2Json");
        }

        //导出设置
        private Excel2JsonOption _option;

        private void OnEnable()
        {
            _option = new Excel2JsonOption();
            _option.Init();
        }

        private void OnDisable()
        {
            _option = null;
        }

        private void OnGUI()
        {
            GUILayout.Label("##############配置导出工具##############");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("---------------------");
            if (GUILayout.Button("Export All", GUILayout.Width(160)))
            {
                _option.Reset();
                _option.explortCsharp = true;
                FuncExporter.Start(_option);
            }

            GUILayout.Label("---------------------");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("---------------------");
            if (GUILayout.Button("Export Single File", GUILayout.Width(160)))
            {
                var excelDir = _option.Rules.excelDirectory;
                var selectedPath =
                    EditorUtility.OpenFilePanelWithFilters(Excel2JsonConfig.Title, excelDir, new[] { "xlsx", "xlsx" });
                //取消操作
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _option.Reset();
                    _option.explortCsharp = true;
                    _option.singleExcelPath = selectedPath;
                    FuncExporter.Start(_option);
                }
            }

            GUILayout.Label("---------------------");
            GUILayout.EndHorizontal();
        }
    }
}