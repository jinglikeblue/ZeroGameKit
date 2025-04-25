using UnityEngine;
using Zero;
using Knight;
using System.Reflection;
using System;
using Jing;
using ZeroGameKit;

namespace Knight
{
    public class G : BaseSingleton<G>
    {
        public SettingModule Setting { get; private set; }

        public AudioModule Audio { get; private set; }


        public void InitModules()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo v in properties)
            {
                if (v.GetValue(this) == null)
                {
                    v.SetValue(this, Activator.CreateInstance(v.PropertyType));
                }
            }
        }

        public void CleanModules()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo v in properties)
            {
                var module = v.GetValue(this) as BaseModule;
                module.Dispose();
                v.SetValue(this, null);
            }
        }


        public override void Destroy()
        {
            CleanModules();
        }



        protected override void Init()
        {
            
        }

    }
}