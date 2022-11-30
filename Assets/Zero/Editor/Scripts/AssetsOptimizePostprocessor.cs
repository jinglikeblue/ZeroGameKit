using UnityEditor;

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
    }
}
