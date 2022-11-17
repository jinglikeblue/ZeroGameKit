using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ZeroEditor
{
    /// <summary>
    /// 资源优化工具类
    /// </summary>
    static class AssetsOptimizeUtility
    {
        static AssetsOptimizeConfigVO _cacheConfigVO;
        static OptimizeSettingModel _settingModel = new OptimizeSettingModel();

        /// <summary>
        /// 配置文件
        /// </summary>
        static public AssetsOptimizeConfigVO Config
        {
            get
            {
                if(_cacheConfigVO == null)
                {
                    _cacheConfigVO = EditorConfigUtil.LoadConfig<AssetsOptimizeConfigVO>(AssetsOptimizeConst.CONFIG_NAME);                    
                    _settingModel.TidySettings(_cacheConfigVO.textureSettings);
                }
                return _cacheConfigVO;                
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        static public void SaveConfig()
        {
            if (null != _cacheConfigVO)
            {
                EditorConfigUtil.SaveConfig(_cacheConfigVO, AssetsOptimizeConst.CONFIG_NAME);
                _settingModel.TidySettings(_cacheConfigVO.textureSettings);
            }
        }

        /// <summary>
        /// 根据配置优化纹理
        /// </summary>
        /// <param name="settings"></param>
        static public void OptimizeTextures()
        {
            OptimizeTexturesFolder("Assets");
        }

        /// <summary>
        /// 根据指定配置优化纹理
        /// </summary>
        /// <param name="settings"></param>
        static public void OptimizeTextures(TextureOptimizeSettingVO setting)
        {
            OptimizeTexturesFolder(setting.folder);
        }

        /// <summary>
        /// 优化文件夹下的所有资源
        /// </summary>
        /// <param name="folder"></param>
        static public void OptimizeTexturesFolder(string folder)
        {
            EditorUtility.DisplayProgressBar(folder, "", 0);

            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                EditorUtility.DisplayProgressBar(folder, FileUtility.GetRelativePath(folder, files[i]), (float)i / files.Length);
                OptimizeTextures(files[i]);
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 优化纹理
        /// </summary>
        /// <param name="filePath"></param>
        static public void OptimizeTextures(string filePath)
        {
            var importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (null == importer)
            {
                //不是纹理资源
                return;
            }

            var setting = _settingModel.FindSetting(filePath);
            if(null == setting)
            {
                //没有对应的优化配置
                return;
            }

            bool isDirty = false;

            if (importer.textureType != setting.textureType)
            {
                importer.textureType = setting.textureType;
                isDirty = true;
            }

            if (importer.npotScale != setting.npotScale)
            {
                importer.npotScale = setting.npotScale;
                isDirty = true;
            }

            if (importer.isReadable != setting.isReadable)
            {
                importer.isReadable = setting.isReadable;
                isDirty = true;
            }

            if (importer.mipmapEnabled != setting.mipmapEnabled)
            {
                importer.mipmapEnabled = setting.mipmapEnabled;
                isDirty = true;
            }

            if (importer.filterMode != setting.filterMode)
            {
                importer.filterMode = setting.filterMode;
                isDirty = true;
            }

            if (SetTextureImporterPlatformSettings(importer, importer.GetDefaultPlatformTextureSettings(), setting.defaultSetting, false))
            {
                isDirty = true;
            }

            if (SetTextureImporterPlatformSettings(importer, importer.GetPlatformTextureSettings(AssetsOptimizeConst.PLATFORM_STANDALONE), setting.standaloneSetting, setting.isOverrideForStandalone))
            {
                isDirty = true;
            }

            if (SetTextureImporterPlatformSettings(importer, importer.GetPlatformTextureSettings(AssetsOptimizeConst.PLATFORM_ANDROID), setting.androidSetting, setting.isOverrideForAndroid))
            {
                isDirty = true;
            }

            if (SetTextureImporterPlatformSettings(importer, importer.GetPlatformTextureSettings(AssetsOptimizeConst.PLATFORM_IOS), setting.iOSSetting, setting.isOverrideForiOS))
            {
                isDirty = true;
            }

            if (isDirty)
            {
                importer.SaveAndReimport();
            }
        }

        static bool SetTextureImporterPlatformSettings(TextureImporter importer, TextureImporterPlatformSettings textureImporterPlatformSettings, TextureOptimizeSettingVO.PlatformSettingVO vo, bool isOverridden)
        {
            bool isDirty = false;

            if(null == textureImporterPlatformSettings)
            {
                textureImporterPlatformSettings = new TextureImporterPlatformSettings();
                textureImporterPlatformSettings.name = vo.name;                
                isDirty = true;
            }

            if(textureImporterPlatformSettings.overridden != isOverridden)
            {
                textureImporterPlatformSettings.overridden = isOverridden;
                isDirty = true;
            }

            if(textureImporterPlatformSettings.maxTextureSize != vo.maxTextureSize)
            {
                textureImporterPlatformSettings.maxTextureSize = vo.maxTextureSize;
                isDirty = true;
            }

            if (textureImporterPlatformSettings.format != vo.format)
            {
                textureImporterPlatformSettings.format = vo.format;
                isDirty = true;
            }

            if (isDirty)
            {
                importer.SetPlatformTextureSettings(textureImporterPlatformSettings);
            }

            return isDirty;
        }
    }
}
