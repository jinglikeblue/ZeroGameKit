using System.IO;
using HybridCLR.Editor.Commands;
using Jing;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// HybridCLR工具类
    /// </summary>
    public static class HybridCLRUtility
    {
        /// <summary>
        /// 拷贝要进行补充元数据的dlls
        /// </summary>
        [MenuItem("Test/HybridCLR/CopyPatchedAOTAssemblyList")]
        public static void CopyPatchedAOTAssemblyList()
        {
            string outputStrippedDir = HybridCLR.Editor.SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget);
            string targetDir = FileUtility.CombineDirs(false, Application.dataPath, "HybridCLRGenerate", "Resources", HybridCLRAotMetadata.AOT_DLL_RESOURCES_DIR);

            if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);

            foreach (var assembly in AOTGenericReferences.PatchedAOTAssemblyList)
            {
                var sourcePath = FileUtility.CombinePaths(outputStrippedDir, assembly);

                var targetPath = FileUtility.CombinePaths(targetDir, assembly + ".bytes");

                FileUtility.CopyFile(sourcePath, targetPath, true);
                Debug.Log(LogColor.Zero2($"拷贝了元数据补充程序集: {targetPath}"));
            }
        }

        /// <summary>
        /// 一键准备HybridCLR
        /// </summary>
        [MenuItem("Test/HybridCLR/OneClickForAll")]
        public static void OneClickForAll()
        {
            //生成一次热更DLL代码
            HotResUtility.GeneateScriptAssembly();
            //生成所有HybridCLR内容
            PrebuildCommand.GenerateAll();
            //拷贝元数据补充需要的程序集
            CopyPatchedAOTAssemblyList();
        }
    }
}