using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 自动生成代码工具
    /// </summary>
    public class AutoGenerateCodeUtility
    {
        /// <summary>
        /// 尝试自动生成资源常量类
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        public static void TryGenerateRClass(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (false == AssetsOptimizeUtility.Config.rClassAutoGenerateSetting.isAutoGenerateEnable)
            {
                //未开启自动生成代码的功能
                return;
            }

            bool isGenerateNeeded;

            do
            {
                isGenerateNeeded = true;
                if (CheckAnyHotResModified(importedAssets))
                {
                    break;
                }

                if (CheckAnyHotResModified(deletedAssets))
                {
                    break;
                }

                if (CheckAnyHotResModified(movedAssets))
                {
                    break;
                }

                if (CheckAnyHotResModified(movedFromAssetPaths))
                {
                    break;
                }

                isGenerateNeeded = false;
            } while (false);

            if (isGenerateNeeded)
            {
                Debug.Log("[Generate] 自动生成资源常量类");
                RightClickEditorMenu.GenerateAssetNames();
            }

            bool CheckAnyHotResModified(string[] assets)
            {
                foreach (var asset in assets)
                {
                    if (asset.StartsWith(ZeroConst.PROJECT_AB_DIR) || asset.StartsWith(ZeroConst.PROJECT_FILES_DIR))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}