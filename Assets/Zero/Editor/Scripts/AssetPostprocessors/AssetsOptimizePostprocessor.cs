using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// Unity导入资源时进行处理
    /// </summary>
    class AssetsOptimizePostprocessor : AssetPostprocessor
    {
        /// <summary>
        /// 导入纹理资源时的处理
        /// </summary>
        public void OnPreprocessTexture()
        {
            //第一次导入时，进行处理
            if (assetImporter.importSettingsMissing)
            {
                AssetsOptimizeUtility.OptimizeTexture(assetImporter as TextureImporter);
            }
        }

        /// <summary>
        /// 在导入模型时调用
        /// </summary>
        public void OnPreprocessModel()
        {
        }

        /// <summary>
        /// 导入声音资源时，进行处理
        /// </summary>
        public void OnPreprocessAudio()
        {
            //第一次导入时，进行处理
            if (assetImporter.importSettingsMissing)
            {
                AssetsOptimizeUtility.OptimizeAudio(assetImporter as AudioImporter);
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                Debug.LogWarning($"[AssetPostprocessor] Reimported: {str}");
            }

            foreach (string str in deletedAssets)
            {
                Debug.LogWarning("[AssetPostprocessor] Deleted: " + str);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                Debug.LogWarning($"[AssetPostprocessor] Moved: {movedFromAssetPaths[i]} => {movedAssets[i]}");
            }

            AutoGenerateCodeUtility.TryGenerateRClass(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
        }
    }
}