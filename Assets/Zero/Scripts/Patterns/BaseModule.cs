using System;

namespace Zero
{
    /// <summary>
    /// 模块类的基类
    /// </summary>
    public abstract class BaseModule : IDisposable
    {
        ~BaseModule()
        {
            Dispose();
        }
        
        /// <summary>
        /// 模块销毁
        /// </summary>
        public abstract void Dispose();
    }
}