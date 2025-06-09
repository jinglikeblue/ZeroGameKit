using System.IO;
using Jing;
using Toolbox.Editor;
using UnityEngine;
using UnityEditor;

namespace ZeroEditor
{
    /// <summary>
    /// 工具条拓展
    /// 备注：
    /// 【Unity自带图标】https://github.com/halak/unity-editor-icons?tab=readme-ov-file
    /// </summary>
    [UnityEditor.InitializeOnLoadAttribute]
    public static class ToolbarExtensions
    {
        private static readonly GUIContent GenerateResDefineScriptIconContent = EditorGUIUtility.IconContent("d_PreTexR");

        private static readonly GUIContent ProjectFolderIconContent = EditorGUIUtility.IconContent("FolderOpened On Icon");

        // 备选图标：「cs Script Icon」
        private static readonly GUIContent GenerateHotScriptsIconContent = EditorGUIUtility.IconContent("cs Script Icon");

        /// <summary>
        /// AssetImport工具图标
        /// </summary>
        private static readonly GUIContent AssetImportToolsIconContent = EditorGUIUtility.IconContent("d_Profiler.NetworkMessages");

        private static GUIStyle _style;

        private static readonly string ProjectFolderPath;

        static ToolbarExtensions()
        {
            ToolbarExpand.OnToolbarGuiLeft += OnToolbarGuiLeft;
            ToolbarExpand.OnToolbarGuiRight += OnToolbarGuiRight;

            GenerateResDefineScriptIconContent.tooltip = "生成资源常量类";
            GenerateHotScriptsIconContent.tooltip = "生成热更程序集(dll)";
            AssetImportToolsIconContent.tooltip = "AssetImport 管理";

            ProjectFolderPath = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(Application.dataPath));

            ProjectFolderIconContent.tooltip = $"访问项目目录：{ProjectFolderPath}";
        }

        private static void OnToolbarGuiLeft()
        {
            if (null == _style)
            {
                _style = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 30,
                    fixedHeight = 20,
                    imagePosition = ImagePosition.ImageOnly,
                };
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(AssetImportToolsIconContent, _style))
            {
                ToolbarEditorMenu.AssetsOptimizeTools();
            }

            if (GUILayout.Button(GenerateHotScriptsIconContent, _style))
            {
                // EditorUtility.DisplayProgressBar("代码生成", "生成热更程序集(dll)", 0);
                RightClickEditorMenu.GenerateDll();
                // EditorUtility.ClearProgressBar();
            }

            //没有开启资源常量类自动生成功能的时候，添加一个手动操作的按钮
            if (!AssetsOptimizeUtility.Config.rClassAutoGenerateSetting.isAutoGenerateEnable)
            {
                if (GUILayout.Button(GenerateResDefineScriptIconContent, _style))
                {
                    EditorUtility.DisplayProgressBar("代码生成", "生成资源常量类", 0);
                    RightClickEditorMenu.GenerateAssetNames();
                    EditorUtility.ClearProgressBar();
                }
            }

            if (GUILayout.Button(ProjectFolderIconContent, _style))
            {
                Application.OpenURL(ProjectFolderPath);
            }
        }

        private static void OnToolbarGuiRight()
        {
            // GUILayout.FlexibleSpace();
            // GUILayout.Space(100);
            // if (GUILayout.Button("设置", GUILayout.Width(40)))
            // {
            //     // GUIUtility.keyboardControl = 0;
            // }
        }
    }
}