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
        static OptimizeSettingModel<TextureOptimizeSettingVO> _textureSettingModel = new OptimizeSettingModel<TextureOptimizeSettingVO>();
        static OptimizeSettingModel<AudioOptimizeSettingVO> _audioSettingModel = new OptimizeSettingModel<AudioOptimizeSettingVO>();

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
                    TidySettings();
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
                TidySettings();
            }
        }

        static private void TidySettings()
        {
            _textureSettingModel.TidySettings(_cacheConfigVO.textureSettings);
            _audioSettingModel.TidySettings(_cacheConfigVO.audioSettings);
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
                OptimizeTexture(files[i]);
            }

            EditorUtility.ClearProgressBar();
        }

        static public void OptimizeTexture(string filePath)
        {
            var importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            OptimizeTexture(importer);
        }

        /// <summary>
        /// 优化纹理
        /// </summary>
        /// <param name="filePath"></param>
        static public void OptimizeTexture(TextureImporter importer)
        {
            if (null == importer)
            {
                //不是纹理资源
                return;
            }

            var filePath = importer.assetPath;

            var setting = _textureSettingModel.FindSetting(filePath);
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

        #region 优化声音文件

        /// <summary>
        /// 根据配置优化声音文件
        /// </summary>
        /// <param name="settings"></param>
        static public void OptimizeAudios()
        {
            OptimizeAudiosFolder("Assets");
        }

        /// <summary>
        /// 根据指定配置优化声音文件
        /// </summary>
        /// <param name="settings"></param>
        static public void OptimizeAudios(TextureOptimizeSettingVO setting)
        {
            OptimizeAudiosFolder(setting.folder);
        }

        /// <summary>
        /// 优化文件夹下的所有资源
        /// </summary>
        /// <param name="folder"></param>
        static public void OptimizeAudiosFolder(string folder)
        {
            EditorUtility.DisplayProgressBar(folder, "", 0);

            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                EditorUtility.DisplayProgressBar(folder, FileUtility.GetRelativePath(folder, files[i]), (float)i / files.Length);
                OptimizeAudio(files[i]);
            }

            EditorUtility.ClearProgressBar();
        }

        static public void OptimizeAudio(string filePath)
        {
            var importer = AssetImporter.GetAtPath(filePath) as AudioImporter;
            OptimizeAudio(importer);
        }

        static public void OptimizeAudio(AudioImporter importer)
        {
            if (null == importer)
            {
                //不是纹理资源
                return;
            }

            var filePath = importer.assetPath;
            AudioOptimizeSettingVO setting = _audioSettingModel.FindSetting(filePath);
            if (null == setting)
            {
                //没有对应的优化配置
                return;
            }

            bool isDirty = false;

            if (importer.forceToMono != setting.forceToMono)
            {
                importer.forceToMono = setting.forceToMono;
                isDirty = true;
            }

            if (importer.loadInBackground != setting.loadInBackground)
            {
                importer.loadInBackground = setting.loadInBackground;
                isDirty = true;
            }

            if (importer.preloadAudioData != setting.preloadAudioData)
            {
                importer.preloadAudioData = setting.preloadAudioData;
                isDirty = true;
            }

            var sampleSettings = importer.defaultSampleSettings;

            if (importer.defaultSampleSettings.loadType != setting.loadType)
            {
                sampleSettings.loadType = setting.loadType;
                isDirty = true;
            }

            if (importer.defaultSampleSettings.compressionFormat != setting.compressionFormat)
            {
                sampleSettings.compressionFormat = setting.compressionFormat;
                isDirty = true;
            }

            if (importer.defaultSampleSettings.quality != (setting.quality / 100f))
            {
                sampleSettings.quality = (setting.quality / 100f);
                isDirty = true;
            }

            if (isDirty)
            {
                importer.defaultSampleSettings = sampleSettings;
                importer.SaveAndReimport();
            }
        }
        #endregion
    }
}
