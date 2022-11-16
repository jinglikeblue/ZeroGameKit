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
            var settings = Config.textureSettings;
            //按照从大到小的顺序，对目录进行排序

            //首先抽取出所有配置的目录中的顶层目录（排除掉嵌套设置的子目录)


            foreach(var setting in settings)
            {
                OptimizeTextures(setting);
            }
        }

        /// <summary>
        /// 根据配置优化纹理
        /// </summary>
        /// <param name="settings"></param>
        static void OptimizeTextures(TextureOptimizeSettingVO setting)
        {            
            var files = Directory.GetFiles(setting.folder, "*.*", SearchOption.AllDirectories);
            for(int i = 0; i < files.Length; i++)
            {                
                var file = FileUtility.StandardizeBackslashSeparator(files[i]);

                EditorUtility.DisplayProgressBar(setting.folder, file, (float)i / files.Length);

                //TODO 检查file是否是子目录配置
                foreach(var tempSetting in Config.textureSettings)
                {
                    if(tempSetting.folder.Contains(setting.folder) && tempSetting.folder.Length > setting.folder.Length)
                    {

                    }
                }

                var importer = AssetImporter.GetAtPath(file) as TextureImporter;
                if(null == importer)
                {
                    continue;
                }

                bool isDirty = false;

                if (importer.textureType != setting.textureType)
                {
                    importer.textureType = setting.textureType;
                    isDirty = true;
                }

                if(importer.npotScale != setting.npotScale)
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
                
                if(SetTextureImporterPlatformSettings(importer, importer.GetDefaultPlatformTextureSettings(), setting.defaultSetting, false))
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

            EditorUtility.ClearProgressBar();
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
