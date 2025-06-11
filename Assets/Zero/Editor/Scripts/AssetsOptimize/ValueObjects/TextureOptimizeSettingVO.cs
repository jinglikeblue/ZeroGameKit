using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 纹理优化配置文件
    /// </summary>
    [ReadOnly]

    [HideReferenceObjectPicker]
    public class TextureOptimizeSettingVO : IOptimizeSettingVO
    {
        /// <summary>
        /// 对应的文件夹(如果另外配置了该文件夹的子目录，则子目录的配置会覆盖该配置)
        /// </summary>
        [LabelText("对应的文件夹"), ReadOnly, FolderPath]
        [GUIColor(0.3f, 0.8f, 0f, 1f)]
        public string folder;

        public string GetFolder()
        {
            return folder;
        }

        [LabelText("Texture Type")]
        public TextureImporterType textureType = TextureImporterType.Sprite;

        [LabelText("Non-Power of 2")]
        public TextureImporterNPOTScale npotScale = TextureImporterNPOTScale.None;

        [LabelText("Read/Write Enabled")]
        public bool isReadable = false;

        [LabelText("Generate Mip Maps")]
        public bool mipmapEnabled = false;

        [LabelText("Filter Mode")]
        public FilterMode filterMode = FilterMode.Point;

        [HideReferenceObjectPicker]
        [BoxGroup("Default Platform Setting")]
        public PlatformSettingVO defaultSetting = new PlatformSettingVO()
        {
            maxTextureSize = 2048,
            format = TextureImporterFormat.Automatic
        };

        [DisplayAsString]
        [HideLabel]
        public string text = "针对平台的配置(勾选Override生效，否则使用上方的默认配置)";

        [TabGroup("Standalone")]
        [LabelText("Override For Standalone")]
        public bool isOverrideForStandalone = false;

        [TabGroup("Standalone")]
        public PlatformSettingVO standaloneSetting = new PlatformSettingVO()
        {
            name = AssetsOptimizeConst.PLATFORM_STANDALONE,
            maxTextureSize = 2048,
            format = TextureImporterFormat.RGBA32
        };

        [TabGroup("Android")]
        [LabelText("Override For Android")]
        public bool isOverrideForAndroid = true;

        [TabGroup("Android")]
        public PlatformSettingVO androidSetting = new PlatformSettingVO()
        {
            name = AssetsOptimizeConst.PLATFORM_ANDROID,
            maxTextureSize = 2048,
            format = TextureImporterFormat.ETC2_RGBA8
        };

        [TabGroup("iOS")]
        [LabelText("Override For iOS")]
        public bool isOverrideForiOS = true;

        [TabGroup("iOS")]
        public PlatformSettingVO iOSSetting = new PlatformSettingVO()
        {
            name = AssetsOptimizeConst.PLATFORM_IOS,
            maxTextureSize = 2048,
            format = TextureImporterFormat.ASTC_6x6
        };
        
        [TabGroup("WebGL")]
        [LabelText("Override For WebGL")]
        public bool isOverrideForWebGL = true;

        [TabGroup("WebGL")]
        public PlatformSettingVO webGLSetting = new PlatformSettingVO()
        {
            name = AssetsOptimizeConst.PLATFORM_IOS,
            maxTextureSize = 2048,
            format = TextureImporterFormat.ASTC_6x6
        };



        [HideLabel, HideReferenceObjectPicker]
        public class PlatformSettingVO
        {
            /// <summary>
            /// 平台名称
            /// </summary>
            [HideInInspector]
            public string name;

            /// <summary>
            /// 最大纹理Size
            /// </summary>     
            [ValueDropdown("TextureSizes")]
            public int maxTextureSize;

            /// <summary>
            /// 纹理压缩格式
            /// </summary>
            public TextureImporterFormat format;

#if UNITY_EDITOR
            static int[] TextureSizes = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
#endif 
        }
    }
}
