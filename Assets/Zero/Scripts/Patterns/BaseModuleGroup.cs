using System;
using System.Reflection;

namespace Zero
{
    /// <summary>
    /// 模块组基类
    /// </summary>
    public abstract class BaseModuleGroup : IDisposable
    {
        protected BaseModuleGroup()
        {
            InitModules();
        }

        ~BaseModuleGroup()
        {
            DisposeModules();
        }

        public void Dispose()
        {
            DisposeModules();
        }

        void InitModules()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo v in properties)
            {
                if(v.GetValue(this) == null)
                {                                        
                    v.SetValue(this, Activator.CreateInstance(v.PropertyType));
                }
            }
        }

        void DisposeModules()
        {            
            PropertyInfo[] properties = GetType().GetProperties();
            foreach(PropertyInfo v in properties)
            {
                var module = v.GetValue(this) as BaseModule;
                module.Dispose();
                v.SetValue(this, null);
            }
        }

        protected void ResetModules()
        {
            DisposeModules();
            InitModules();
        }
    }
}