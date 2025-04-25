using UnityEngine;
using Zero;
using ZeroHot;
using System.Reflection;
using ZeroGameKit;
using System;
using Jing;

namespace Sokoban
{
    public class SokobanGlobal : BaseSingleton<SokobanGlobal>
    {

        /// <summary>
        /// 菜单
        /// </summary>
        public MenuModule Menu { get; private set; }

        /// <summary>
        /// 通知
        /// </summary>
        public NoticeModule Notice { get; private set; }

        /// <summary>
        /// 音频
        /// </summary>
        public AudioModule Audio { get; private set; }

        /// <summary>
        /// 关卡
        /// </summary>
        public LevelModule Level { get; private set; }

        protected override void Init()
        {
            
        }

        public void InitModules()
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

        public void CleanModules()
        {            
            PropertyInfo[] properties = GetType().GetProperties();
            foreach(PropertyInfo v in properties)
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
    }
}