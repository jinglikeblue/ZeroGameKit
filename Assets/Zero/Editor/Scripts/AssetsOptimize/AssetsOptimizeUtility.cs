using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroEditor
{
    /// <summary>
    /// 资源优化工具类
    /// </summary>
    static class AssetsOptimizeUtility
    {
        static AssetsOptimizeConfigVO _cacheConfigVO;

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
            }
        }
    }
}
