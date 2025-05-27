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
        private static readonly GUIContent GenerateScriptIconContent = EditorGUIUtility.IconContent("d_PreTexR");
        private static readonly GUIContent ProjectFolderIconContent = EditorGUIUtility.IconContent("FolderOpened On Icon");

        private static GUIStyle _style;

        private static readonly string ProjectFolderPath;

        static ToolbarExtensions()
        {
            ToolbarExpand.OnToolbarGuiLeft += OnToolbarGuiLeft;
            ToolbarExpand.OnToolbarGuiRight += OnToolbarGuiRight;

            GenerateScriptIconContent.tooltip = "生成资源常量类";

            ProjectFolderPath = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(Application.dataPath));

            ProjectFolderIconContent.tooltip = ProjectFolderPath;
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

            if (GUILayout.Button(GenerateScriptIconContent, _style))
            {
                EditorUtility.DisplayProgressBar("代码生成", "生成资源常量类", 0);
                RightClickEditorMenu.GenerateAssetNames();
                EditorUtility.ClearProgressBar();
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