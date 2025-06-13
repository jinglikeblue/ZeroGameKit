using System.IO;
using Jing;
using UnityEditor;
using UnityEngine;
using Zero;
using ZeroEditor.IOS;

namespace ZeroEditor
{
    /// <summary>
    /// Zero框架编辑器菜单
    /// </summary>
    public class ToolbarEditorMenu
    {
        [MenuItem("Zero/热更构建", false, 0)]
        public static void HotResBuild()
        {
            BuildHotResEditorWin.Open();
        }

        [MenuItem("Zero/HybridCLR一键部署", false, 100)]
        public static void HybridCLROneClickForAll()
        {
            HybridCLREditorUtility.OneClickForAll();
        }

        #region Platform

        [MenuItem("Zero/Platform/iOS构建自动化配置", false, 200)]
        public static void IosProjectInit()
        {
            IOS.IOSProjectInitEditorWin.Open();
        }

        #endregion

        #region 工具

        [MenuItem("Zero/工具/位图字体创建", false, 300)]
        public static void CreateBitmapFontGUITools()
        {
            BitmapFontCreaterMenu.CreateBitmapFontGUITools();
        }

        [MenuItem("Zero/工具/SpriteAtlas 管理", false, 301)]
        public static void SpriteAtlasTools()
        {
            SpriteAtlasToolsEditorWin.Open();
        }

        [MenuItem("Zero/工具/AssetImporter 管理", false, 302)]
        public static void AssetsOptimizeTools()
        {
            AssetsOptimizeEditorWindow.Open();
        }

        [MenuItem("Zero/工具/冗余资源管理", false, 303)]
        public static void RedundancyResourcesCleanTools()
        {
            RedundancyResourcesCleanToolsEditorWin.Open();
        }

        [MenuItem("Zero/工具/字体替换工具", false, 304)]
        public static void FontSwitchTools()
        {
            FontSwitchEditorWin.Open();
        }

        #endregion

        #region 其它

        [MenuItem("Zero/其它/配置文件编辑", false, 400)]
        public static void Configs()
        {
            HotConfigEditorWin.Open("配置文件编辑");
        }

        [MenuItem("Zero/其它/自动生成代码", false, 401)]
        public static void GenerateCode()
        {
            GenerateCodeEditorWin.Open();
        }

        [MenuItem("Zero/其它/项目工程/创建分身", false, 402)]
        public static void CreateShadowProject()
        {
            var projectDirInfo = new DirectoryInfo(Application.dataPath).Parent;

            var projectPath = projectDirInfo.FullName;
            var projectName = projectDirInfo.Name;

            Debug.Log($"当前项目: [{projectName}]({projectPath})");

#if UNITY_EDITOR_WIN
            var shadowProjectDir = EditorUtility.OpenFolderPanel("保存位置", $"{projectDirInfo.Parent.FullName}", $"{projectName}_Shadow");
            ZeroEditorUtility.CreateShadowProject(projectPath, shadowProjectDir);
#else
            EditorUtility.DisplayDialog("错误提示", "此功能暂不支持该平台！", "确定");
#endif
        }

        #endregion

        #region 调试

        [MenuItem("Zero/调试/清理[Caches]目录", false, 500)]
        public static void ClearCachesDir()
        {
            var cacheDir = new DirectoryInfo(ZeroConst.PERSISTENT_DATA_PATH);
            if (cacheDir.Exists)
            {
                cacheDir.Delete(true);
            }
        }

        [MenuItem("Zero/调试/打开[Application.temporaryCachePath]目录", false, 501)]
        public static void OpenTemporaryCacheDir()
        {
            Application.OpenURL(Application.temporaryCachePath);
        }

        [MenuItem("Zero/调试/GC", false, 502)]
        public static void GC()
        {
            Runtime.GC();
        }
        
        [MenuItem("Zero/调试/清理Editor下的SpriteAtlas缓存", false, 503)]
        public static void ClearSpriteAtlasLibraryCache()
        {
            SpriteAtlasToolsUtility.ClearLibraryCache();
        }

        #endregion


        #region 目录访问

        [MenuItem("Zero/文件夹访问/工程目录", false, 600)]
        public static void OpenProjectFolder()
        {
            Application.OpenURL(ZeroEditorConst.PROJECT_PATH);
        }

        [MenuItem("Zero/文件夹访问/运行时可读写目录", false, 601)]
        public static void OpenPersistentDataFolder()
        {
            if (Directory.Exists(ZeroConst.PERSISTENT_DATA_PATH))
            {
                Application.OpenURL(ZeroConst.PERSISTENT_DATA_PATH);
            }
        }

        [MenuItem("Zero/文件夹访问/热更发布目录", false, 602)]
        public static void OpenHotReleaseFolder()
        {
            var dir = FileUtility.CombineDirs(false, ZeroEditorConst.PROJECT_PATH, ZeroConst.ZERO_LIBRARY_DIR, "Release");
            if (Directory.Exists(dir))
            {
                Application.OpenURL(dir);
            }
        }

        [MenuItem("Zero/文件夹访问/内嵌资源目录", false, 603)]
        public static void OpenBuiltinResFolder()
        {
            if (Directory.Exists(ZeroConst.BuiltinResRootFolder))
            {
                Application.OpenURL(ZeroConst.BuiltinResRootFolder);
            }
        }

        #endregion

        [MenuItem("Zero/About", false, 99999)]
        public static void Document()
        {
            AboutEditorWin.Open();
        }
    }
}