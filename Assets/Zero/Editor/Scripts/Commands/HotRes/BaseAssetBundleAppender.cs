using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Zero
{
    /// <summary>
    /// AssetBundle额外附加的基类。
    /// 继承该类并实现方法，则在进行AssetBundle打包时
    /// </summary>
    public abstract class BaseAssetBundleAppender
    {
        /// <summary>
        /// 子类实现该方法，返回需要附加的AB包内容
        /// </summary>
        /// <returns></returns>
        public abstract AssetBundleBuild[] AssetBundles();        
    }
}
