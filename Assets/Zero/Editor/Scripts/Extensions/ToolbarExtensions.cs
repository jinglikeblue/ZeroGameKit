using System;
using System.Collections.Generic;
using System.IO;
using Jing;
using Toolbox.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
        /*
         * 备用图标
         * 位图字体：TextMesh Icon，GUIText Icon
         * 冗余资源检查：d_ViewToolZoom，d_MeshRenderer Icon，d_RenderTexture Icon，d_SpriteRenderer Icon，d_TilemapRenderer Icon
         */

        /// <summary>
        /// d_TextScriptImporter Icon
        /// d_PreTexR
        /// </summary>
        private static readonly GUIContent GenerateResDefineScriptIconContent = EditorGUIUtility.IconContent("d_TextScriptImporter Icon");

        private static readonly GUIContent ProjectFolderIconContent = EditorGUIUtility.IconContent("FolderOpened On Icon");

        // 备选图标：「cs Script Icon」[d_Assembly Icon]
        private static readonly GUIContent GenerateHotScriptsIconContent = EditorGUIUtility.IconContent("AssemblyDefinitionAsset Icon");

        /// <summary>
        /// AssetImport工具图标
        /// </summary>
        private static readonly GUIContent AssetImportToolsIconContent = EditorGUIUtility.IconContent("d_Profiler.NetworkMessages");

        /// <summary>
        /// 切换场景图标
        /// </summary>
        private static readonly GUIContent SwitchToStartupSceneIconContent = EditorGUIUtility.IconContent("d_SceneAsset Icon");

        /// <summary>
        /// SpriteAtlas工具图标
        /// </summary>
        private static readonly GUIContent SpriteAtlasToolsIconContent = EditorGUIUtility.IconContent("d_SpriteAtlasAsset Icon");

        /// <summary>
        /// 字体管理工具
        /// </summary>
        private static readonly GUIContent FontToolIconContent = EditorGUIUtility.IconContent("d_Font Icon");

        /// <summary>
        /// 位图字体工具
        /// </summary>
        private static readonly GUIContent BitmapFontIconContent = EditorGUIUtility.IconContent("GUIText Icon");

        /// <summary>
        /// 冗余资源检查工具图标
        /// d_TilemapRenderer Icon
        /// </summary>
        private static readonly GUIContent RedundancyResourcesCleanToolsIconContent = EditorGUIUtility.IconContent("d_TreeEditor.Trash");
        
        /// <summary>
        /// 热更资源构建图标
        /// d_FilterByType，d_PreMatCylinder，d_BlendTree Icon，Skybox Icon，d_CloudConnect，d_Profiler.GlobalIllumination
        /// </summary>
        private static readonly GUIContent HotResBuildIconContent = EditorGUIUtility.IconContent("d_Profiler.GlobalIllumination");

        private static GUIStyle _style;

        private static readonly string ProjectFolderPath;

        // private static readonly Dictionary<string, Action>  _rightClickMenu = new Dictionary<string, Action>
        // {
        //     { "生成资源常量类", RightClickEditorMenu.GenerateAssetNames },
        //     { "生成热更程序集(dll)", RightClickEditorMenu.GenerateDll },
        //     { "生成代码", RightClickEditorMenu.GenerateCode },
        //     { "生成代码(C#)", RightClickEditorMenu.GenerateCodeCSharp },
        //     { "生成代码(Lua)", RightClickEditorMenu.GenerateCodeLua },
        //     { "生成代码(C#&Lua)", RightClickEditorMenu.GenerateCode}
        // }

        static ToolbarExtensions()
        {
            ToolbarExpand.OnToolbarGuiLeft += OnToolbarGuiLeft;
            ToolbarExpand.OnToolbarGuiRight += OnToolbarGuiRight;

            GenerateResDefineScriptIconContent.tooltip = "生成资源常量类";
            GenerateHotScriptsIconContent.tooltip = "生成热更程序集(dll)";
            AssetImportToolsIconContent.tooltip = "AssetImport 管理";

            ProjectFolderPath = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(Application.dataPath));

            ProjectFolderIconContent.tooltip = $"{ProjectFolderPath}";

            SwitchToStartupSceneIconContent.tooltip = $"切换场景至: StartupScene";
            SpriteAtlasToolsIconContent.tooltip = "SpriteAtlas 管理";
            FontToolIconContent.tooltip = "字体替换";
            RedundancyResourcesCleanToolsIconContent.tooltip = "冗余资源检查";
            BitmapFontIconContent.tooltip = "位图字体创建";
            HotResBuildIconContent.tooltip = "热更构建";
        }


        /// <summary>
        /// TODO 优化展示，通过数组配置来展示，不然代码写着太麻烦
        /// </summary>
        private static void OnToolbarGuiLeft()
        {
            if (null == _style)
            {
                _style = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 30,
                    fixedHeight = 20,
                    imagePosition = ImagePosition.ImageOnly,
                    alignment = TextAnchor.MiddleCenter,
                };
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(ProjectFolderIconContent, _style))
            {
                Application.OpenURL(ProjectFolderPath);
            }

            GUILayout.Label("|");
            
            if (GUILayout.Button(HotResBuildIconContent, _style))
            {
                ToolbarEditorMenu.HotResBuild();
            }
            
            GUILayout.Label("|");

            if (GUILayout.Button(GenerateHotScriptsIconContent, _style))
            {
                // EditorUtility.DisplayProgressBar("代码生成", "生成热更程序集(dll)", 0);
                RightClickEditorMenu.GenerateDll();
                // EditorUtility.ClearProgressBar();
            }

            //没有开启资源常量类自动生成功能的时候，添加一个手动操作的按钮
            // if (!AssetsOptimizeUtility.Config.rClassAutoGenerateSetting.isAutoGenerateEnable)
            // {
            if (GUILayout.Button(GenerateResDefineScriptIconContent, _style))
            {
                EditorUtility.DisplayProgressBar("代码生成", "生成资源常量类", 0);
                RightClickEditorMenu.GenerateAssetNames();
                EditorUtility.ClearProgressBar();
            }
            // }

            GUILayout.Label("|");

            if (GUILayout.Button(AssetImportToolsIconContent, _style))
            {
                ToolbarEditorMenu.AssetsOptimizeTools();
            }

            if (GUILayout.Button(SpriteAtlasToolsIconContent, _style))
            {
                ToolbarEditorMenu.SpriteAtlasTools();
            }

            if (GUILayout.Button(RedundancyResourcesCleanToolsIconContent, _style))
            {
                ToolbarEditorMenu.RedundancyResourcesCleanTools();
            }

            if (GUILayout.Button(FontToolIconContent, _style))
            {
                ToolbarEditorMenu.FontSwitchTools();
            }

            if (GUILayout.Button(BitmapFontIconContent, _style))
            {
                ToolbarEditorMenu.CreateBitmapFontGUITools();
            }

            GUILayout.Label("|");

            if (GUILayout.Button(SwitchToStartupSceneIconContent, _style))
            {
                const string startupScenePath = "Assets/StartupScene.unity";
                var currentScenePath = SceneManager.GetActiveScene().path;
                if (!currentScenePath.Equals(startupScenePath))
                {
                    if (EditorUtility.DisplayDialog("提示", "切换场景到StartupScene，会丢失当前场景未保存的修改，是否切换？", "确认", "取消"))
                    {
                        EditorSceneManager.OpenScene(startupScenePath);
                    }
                }
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