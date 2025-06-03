using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Jing;
using UnityEditor;
using UnityEngine;
using Zero;
using ZeroEditor;

/// <summary>
/// 自定义右键菜单
/// </summary>
public static class RightClickEditorMenu
{
    [MenuItem("Assets/Zero/生成DLL（并拷贝到内嵌资源目录)", false, 0)]
    public static async void GenerateDll()
    {
        try
        {
            EditorUtility.DisplayProgressBar("dll生成", "正在生成", 0f);
            await HotResEditorUtility.GenerateScriptAssembly();
            EditorUtility.DisplayProgressBar("dll生成", "拷贝到StreamingAssets", 0.9f);
            var builtinFolder = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, ZeroConst.DLL_DIR_NAME);
            FileUtility.CopyDir(ZeroEditorConst.DLL_PUBLISH_DIR, builtinFolder);
            EditorUtility.ClearProgressBar();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            EditorUtility.ClearProgressBar();
        }
    }

    #region 工具/SpriteAtlas Tools

    [MenuItem("Assets/Zero/工具/SpriteAtlas Tools/添加目录到SpriteAtlas配置", false, 1)]
    static void SpriteAtlasAdd()
    {
        if (Selection.objects.Length != 1)
        {
            EditorUtility.DisplayDialog("错误", "仅支持[单选]的[文件夹]", "OK");
            return;
        }

        var obj = Selection.objects[0];
        var assetPath = AssetDatabase.GetAssetPath(obj);
        if (false == Directory.Exists(assetPath))
        {
            EditorUtility.DisplayDialog("错误", "仅支持[单选]的[文件夹]", "OK");
            return;
        }

        SpriteAtlasToolsUtility.AddSpriteAtlas(assetPath, false);
    }

    [MenuItem("Assets/Zero/工具/SpriteAtlas Tools/构建全部 SpriteAtlas", false, 2)]
    static void SpriteAtlasBuildAll()
    {
        SpriteAtlasToolsUtility.BuildAll();
    }

    [MenuItem("Assets/Zero/工具/SpriteAtlas Tools/SpriteAtlas Tools", false, 3)]
    static void SpriteAtlasEditorWin()
    {
        SpriteAtlasToolsEditorWin.Open();
    }

    #endregion

    #region 工具/位图字体

    [MenuItem("Assets/Zero/工具/位图字体/直接创建/使用Png图片的名称作为字符源", false, 1)]
    static void CreateBitmapFontUsePNGFileName()
    {
        BitmapFontCreaterMenu.CreateBitmapFontUsePNGFileName();
    }

    [MenuItem("Assets/Zero/工具/位图字体/直接创建/使用「chars.txt」作为字符源", false, 2)]
    static void CreateBitmapFontUseCharsTxt()
    {
        BitmapFontCreaterMenu.CreateBitmapFontUseCharsTxt();
    }

    [MenuItem("Assets/Zero/工具/位图字体/使用GUI创建", false, 3)]
    static void CreateBitmapFontGUI()
    {
        BitmapFontCreaterMenu.CreateBitmapFontGUI();
    }

    #endregion

    [MenuItem("Assets/Zero/生成资源常量类", false, 100)]
    public static void GenerateAssetNames()
    {
        new GenerateRClassCommand().Excute();
        var findCmd = new FindAssetBundlesCommand(false);
        findCmd.onFinished += (cmd, list) =>
        {
            var startTime = DateTime.Now;
            new GenerateABClassCommand(list).Excute();
            AssetDatabase.Refresh();

            var tn = DateTime.Now - startTime;
            
            Debug.Log(LogColor.Zero1($"生成代码: {GenerateRClassCommand.OUTPUT_FILE}"));
            Debug.Log(LogColor.Zero1($"生成代码: {GenerateABClassCommand.OUTPUT_FILE}"));
            Debug.Log(LogColor.Zero1($"生成完毕! 耗时:{(long)tn.TotalMilliseconds}ms"));
        };
        findCmd.Excute();
    }

    [MenuItem("Assets/Zero/调试/选中文件的Importer信息")]
    static void SelectionImporterInfo()
    {
        for (var i = 0; i < Selection.objects.Length; i++)
        {
            var obj = Selection.objects[i];
            var path = AssetDatabase.GetAssetPath(obj);
            var importer = AssetImporter.GetAtPath(path);
            Debug.Log(LogColor.Zero1($"Name: {obj.name}, ObjectType: {obj.GetType()}, ImporterType: {importer.GetType().Name}, Path: {path}"));
        }
    }
}