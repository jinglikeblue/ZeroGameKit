//该类由 [Zero → 自动生成代码 → Assets资源名生成] 工具自动创建
using System;
using UnityEngine;
using Zero;

namespace ZeroHot
{
    /// <summary>
    /// 自动视图注册器
    /// </summary>
    class ViewAutoRegister
    {
        static ViewAutoRegister _ins;

        /// <summary>
        /// 是否已注册
        /// </summary>
        public static bool IsRegistered {
            get
            {
                return _ins == null ? false : true;
            }
        }

        /// <summary>
        /// 进行注册
        /// </summary>
        public static void Register()
        {
            if(null == _ins)
            {
                _ins = new ViewAutoRegister();                
            }
        }

        /// <summary>
        /// 注册的用来
        /// </summary>
        string[] namespaceList = new string[] {
            "Roushan.",
            "ZeroGameKit.",
            "Example.",
        };

        private ViewAutoRegister()
        {
            R("root_assets.ab", "ILContent");
            R("commons.ab", "MsgWin");
            R("examples.ab", "MainStartupPanel");
            R("examples/audio_device.ab", "AudioDeviceExampleWin");
            R("examples/bitmapfont.ab", "BitmapFontExampleWin");
            R("examples/menus.ab", "MenuPanel");
            R("examples/performance.ab", "CallNativeCodePerformanceWin");
            R("examples/performance.ab", "FibonacciPerformanceWin");
            R("examples/roushan.ab", "Block");
            R("examples/roushan.ab", "GamePanel");
            R("examples/roushan.ab", "GameStage");
            R("examples/roushan.ab", "HelpWin");
            R("examples/roushan.ab", "StartupPanel");
            R("examples/textures.ab", "TexturesWin");
            R("examples/utility.ab", "MD5ExampleWin");
            R("examples/cross_depend_test/a.ab", "A");
            R("examples/cross_depend_test/a.ab", "C");
            R("examples/cross_depend_test/b.ab", "B");
        }

        void R(string abName, string viewName)
        {
            bool isRegisterSuccess = false;
            foreach(var ns in namespaceList)
            {
                var typeName = ns + viewName;
                var t = Type.GetType(typeName);
                if (t != null)
                {
                    ViewFactory.Register(abName, t.Name, t);
                    isRegisterSuccess = true;
                    break;
                }
            }

            if (!isRegisterSuccess)
            {
                Debug.Log(Log.Orange("[{0}/{1}.prefab]没有找到匹配的类，请检查是否存在「{2}.cs」，或者「自动生成代码」配置的命名空间是否正确!!!", abName, viewName, viewName));
            }
        }
    }
}
