using Toolbox.Editor;
using UnityEngine;
using UnityEditor;

namespace ZeroEditor
{
    /// <summary>
    /// 工具条拓展
    /// </summary>
    [UnityEditor.InitializeOnLoadAttribute]
    public static class ToolbarExtensions
    {
        private static readonly GUIContent GenerateScriptIconContent = EditorGUIUtility.IconContent("d_PreTexR");

        static ToolbarExtensions()
        {
            ToolbarExpand.OnToolbarGuiLeft += OnToolbarGuiLeft;
            ToolbarExpand.OnToolbarGuiRight += OnToolbarGuiRight;
        }

        private static void OnToolbarGuiLeft()
        {
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(GenerateScriptIconContent))
            {
                EditorUtility.DisplayProgressBar("代码生成", "生成资源常量类", 0);
                RightClickEditorMenu.GenerateAssetNames();
                EditorUtility.ClearProgressBar();
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