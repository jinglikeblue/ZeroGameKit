using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroEditor
{
    /// <summary>
    /// 编辑器代码相关设置
    /// </summary>
    static class ZeroEditorSettings
    {
        #region 热更相关

        /// <summary>
        /// 是否启用AssetBundle资源附加器功能
        /// </summary>
        public const bool ASSET_BUNDLE_APPENDER_ENABLE = true;

        /// <summary>
        /// 是否启用AssetBundle优化资源交叉引用的情况。
        /// PS：开启后可以减小AB包资源总大小
        /// </summary>
        public const bool CREATE_CROSS_ASSET_BUNDLE_ENABLE = true;

        #endregion
    }
}
