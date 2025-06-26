using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZeroEditor;

public class TestAssetBundle
{
    [MenuItem("Test/AssetBundle/Build")]
    public static void TestAssetBundleBuild()
    {
        var abbList = new AssetBundleBuild[1];
        abbList[0] = new AssetBundleBuild();
        abbList[0].assetBundleName = "test_ab.ab";
        abbList[0].assetNames = new string[] { "Assets/$UnitTest/URPAssetBundleTest/TestAsset.prefab" };

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        var assetBundleManifest = BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, abbList, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);

        if (null != assetBundleManifest)
        {
            Debug.Log($"打包完成");
            ZeroEditorUtility.OpenDirectory(Application.streamingAssetsPath);
        }
        else
        {
            Debug.LogError("AssetBundle打包失败!");
        }
    }
}
