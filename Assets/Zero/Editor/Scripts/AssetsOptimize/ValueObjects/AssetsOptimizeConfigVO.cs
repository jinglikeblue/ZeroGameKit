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
        public bool isTextureOptimizeEnable;               
        public List<TextureOptimizeSettingVO> textureSetting = new List<TextureOptimizeSettingVO>();

        public bool isAudioOptimizeEnable;
        public List<AudioOptimizeSettingVO> audioSetting = new List<AudioOptimizeSettingVO>();
    }
}
