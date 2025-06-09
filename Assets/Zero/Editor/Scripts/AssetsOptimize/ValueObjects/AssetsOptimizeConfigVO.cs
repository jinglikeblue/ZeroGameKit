using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace ZeroEditor
{    
    /// <summary>
    /// 资源优化工具配置数据对象
    /// </summary>
    class AssetsOptimizeConfigVO
    {
        /// <summary>
        /// 是否允许再导入纹理时自动进行优化
        /// </summary>
        public bool isTextureOptimizeEnable;               
        public List<TextureOptimizeSettingVO> textureSettings = new List<TextureOptimizeSettingVO>();

        /// <summary>
        /// 是否允许在导入音频时自动进行优化
        /// </summary>
        public bool isAudioOptimizeEnable;
        public List<AudioOptimizeSettingVO> audioSettings = new List<AudioOptimizeSettingVO>();

        /// <summary>
        /// 资源常量自动生成
        /// </summary>
        public RClassAutoGenerateSettingVO rClassAutoGenerateSetting = new RClassAutoGenerateSettingVO();
    }
}
