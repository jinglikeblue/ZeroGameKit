using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// 视图注册属性
    /// 当项目中有多个AView子类有同样的名字时，通过该标签可以指定关联的prefab。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewRegisterAttribute : Attribute
    {
        /// <summary>
        /// 预制体路径(参考AB.cs类)
        /// </summary>
        public string prefabPath { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabPath">预制体路径(参考AB.cs类)</param>
        public ViewRegisterAttribute(string prefabPath)
        {            
            if (prefabPath.StartsWith(ZeroConst.PROJECT_AB_DIR))
            {
                prefabPath = prefabPath.Replace(ZeroConst.PROJECT_AB_DIR, "");
                prefabPath = FileUtility.RemoveStartPathSeparator(prefabPath);
            }
            this.prefabPath = prefabPath;
        }
    }
}
