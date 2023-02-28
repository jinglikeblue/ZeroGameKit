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
            "Sokoban.",
            "Knight.",
        };

        private ViewAutoRegister()
        {
            R("root_assets.ab", "ILContent");
            R("commons.ab", "MsgWin");
            R("examples.ab", "MainStartupPanel");
            R("examples/audio_device.ab", "AudioDeviceExampleWin");
            R("examples/bitmapfont.ab", "BitmapFontExampleWin");
            R("examples/framework.ab", "ClockView");
            R("examples/framework.ab", "CoroutineProxyExampleWin");
            R("examples/framework.ab", "CoroutinesQueueExampleWin");
            R("examples/framework.ab", "HotFilesExampleWin");
            R("examples/framework.ab", "ResMgrExampleWin");
            R("examples/framework.ab", "ResUpdateExampleWin");
            R("examples/framework.ab", "ScreenSafeAreaExampleWin");
            R("examples/framework.ab", "TimerExampleWin");
            R("examples/framework.ab", "ViewFactoryExampleWin");
            R("examples/framework.ab", "ZeroEventListenerExampleWin");
            R("examples/framework.ab", "ZeroUIExampleWin");
            R("examples/future.ab", "StreamingAssetsLoadFutureWin");
            R("examples/knight.ab", "KnightLoadingPanel");
            R("examples/knight.ab", "KnightMenuPanel");
            R("examples/knight.ab", "KnightSettingWin");
            R("examples/menus.ab", "MenuPanel");
            R("examples/network.ab", "KcpExampleWin");
            R("examples/network.ab", "TcpExampleWin");
            R("examples/network.ab", "UdpExampleWin");
            R("examples/network.ab", "WebSocketExampleWin");
            R("examples/performance.ab", "CallNativeCodePerformanceWin");
            R("examples/performance.ab", "FibonacciPerformanceWin");
            R("examples/roushan.ab", "Block");
            R("examples/roushan.ab", "GamePanel");
            R("examples/roushan.ab", "GameStage");
            R("examples/roushan.ab", "HelpWin");
            R("examples/roushan.ab", "StartupPanel");
            R("examples/textures.ab", "TexturesWin");
            R("examples/turbocharged_scroll_list.ab", "GridScrollListDemoPanel");
            R("examples/turbocharged_scroll_list.ab", "HorizontalScrollListDemoPanel");
            R("examples/turbocharged_scroll_list.ab", "ScrollListDemoMenuPanel");
            R("examples/turbocharged_scroll_list.ab", "VerticalScrollListDemoPanel");
            R("examples/uniwebview.ab", "UniWebViewExampleWin");
            R("examples/utility.ab", "CryptoExampleWin");
            R("examples/video.ab", "VideoExampleWin");
            R("examples/cross_depend_test/a.ab", "A");
            R("examples/cross_depend_test/a.ab", "C");
            R("examples/cross_depend_test/b.ab", "B");
            R("examples/knight/game.ab", "KnightGamePanel");
            R("examples/knight/game.ab", "KnightGameStage");
            R("examples/sokoban/prefabs.ab", "CreditsWin");
            R("examples/sokoban/prefabs.ab", "LevelSelectWin");
            R("examples/sokoban/prefabs.ab", "LoadingWin");
            R("examples/sokoban/prefabs.ab", "SokobanGamePanel");
            R("examples/sokoban/prefabs.ab", "SokobanGameStage");
            R("examples/sokoban/prefabs.ab", "SokobanMenuPanel");
            R("examples/sokoban/prefabs.ab", "SokobanMsgWin");
            R("examples/sokoban/prefabs/game.ab", "BangEffect");
            R("examples/sokoban/prefabs/game.ab", "Block");
            R("examples/sokoban/prefabs/game.ab", "Box");
            R("examples/sokoban/prefabs/game.ab", "Role");
            R("examples/sokoban/prefabs/game.ab", "Target");
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
