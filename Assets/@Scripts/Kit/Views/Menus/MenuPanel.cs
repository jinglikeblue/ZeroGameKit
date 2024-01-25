using Example;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroHot;

namespace ZeroGameKit
{
    public class MenuPanel : AView
    {
        const string GROUP_FUTURE = "研发中功能...";
        const string GROUP_DEMO = "DEMO";
        const string GROUP_IOS = "iOS系统";
        const string GROUP_ANDROID = "Android系统";
        const string GROUP_FRAMEWORK = "框架";
        const string GROUP_DOTWEEN = "DoTween使用";
        const string GROUP_UNIWEBVIEW = "UniWebView使用";
        const string GROUP_TURBOCHARGESCROLLLIST = "TurbochargeScrollList使用";
        const string GROUP_NET = "网络";
        const string GROUP_FILE = "文件";
        const string GROUP_UTILS = "实用工具";
        const string GROUP_EXTEND = "扩展类";
        const string GROUP_BITMAPFONT = "BitmapFont使用";
        const string GROUP_AUDIO = "使用AudioDevice处理音频";
        const string GROUP_PERFORMANCE = "性能测试";
        const string GROUP_AI = "AI 人工智能";
        const string GROUP_UNITY = "Unity用例";
        const string GROUP_VIDEO = "视频";
        const string GROUP_DEBUG = "调试";


        Dictionary<string, MenuButtonGroupItem> _groupItemDic = new Dictionary<string, MenuButtonGroupItem>();

        GameObject buttonGroupItem;
        Transform content;
        Toggle toggleShowTodo;

        protected override void OnDisable()
        {
            base.OnDisable();
            toggleShowTodo.onValueChanged.RemoveListener(OnToggleShowTodoValueChanged);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            toggleShowTodo.onValueChanged.AddListener(OnToggleShowTodoValueChanged);
        }

        void ClearCache()
        {
            var cacheDir = new DirectoryInfo(ZeroConst.PERSISTENT_DATA_PATH);
            if (cacheDir.Exists)
            {
                cacheDir.Delete(true);
            }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            StageMgr.Ins.Clear();

            buttonGroupItem.SetActive(false);

            AddBtn(GROUP_DEBUG, "测试包下载地址", () => { Application.OpenURL("https://tt.appc02.com/y8b6"); });
            AddBtn(GROUP_DEBUG, "清空缓存", ClearCache);
            AddBtn(GROUP_DEBUG, "GC", ResMgr.Ins.DoGC);            

            AddBtn(GROUP_FUTURE, "加载StreamingAssets", StreamingAssetsLoadFuture.Start);

            AddBtn(GROUP_DEMO, "DEMO_2D物理游戏", RoushanExample.Start);
            AddBtn(GROUP_DEMO, "DEMO_推箱子游戏", SokobanExample.Start);
            AddBtn(GROUP_DEMO, "DEMO_3D", KnightExample.Start);
            AddBtn(GROUP_DEMO, "DEMO_帧同步游戏", FrameSyncGameExample.Start);

            AddBtn(GROUP_PERFORMANCE, "运算性能", PerformanceExample.Calculate);
            AddBtn(GROUP_PERFORMANCE, "与主工程交互性能", PerformanceExample.CallNative);

            AddBtn(GROUP_IOS, "iOS交互", IOSBridgeExample.Start);

            AddBtn(GROUP_ANDROID, "Android交互", AndroidBridgeExample.Start);
            AddBtn(GROUP_DOTWEEN, "DoTween", DoTweenExample.Start);
            AddBtn(GROUP_UNIWEBVIEW, "网页浏览", UniWebViewExample.Start);
            AddBtn(GROUP_FRAMEWORK, "单例使用示例", SingletonClassExample.Start);
            AddBtn(GROUP_FRAMEWORK, "消息窗口", MsgWinExample.Start);
            AddBtn(GROUP_FRAMEWORK, "配置使用示例", ConfigExample.Start);
            AddBtn(GROUP_FRAMEWORK, "定点数示例", FixedPointNumberExample.Start);
            AddBtn(GROUP_FRAMEWORK, "高性能文本描边", TextOutlineExample.Start);

            AddBtn(GROUP_TURBOCHARGESCROLLLIST, "高性能列表", TurbochargedScrollListExample.Start);

            AddBtn(GROUP_NET, "Tcp通信", TcpExample.Start);
            AddBtn(GROUP_NET, "Udp通信", UdpExample.Start);
            AddBtn(GROUP_NET, "WebSocket通信", WebSocketExample.Start);
            AddBtn(GROUP_NET, "Web请求", WebExample.Start);
            AddBtn(GROUP_NET, "网络文件下载", DownloadFileExample.Start);
            AddBtn(GROUP_NET, "Protobuf使用", ProtoBufExample.Start);
            AddBtn(GROUP_NET, "Kcp使用", KcpExample.Start);


            AddBtn(GROUP_FILE, "字节数组操作", ByteArrayExample.Start);
            AddBtn(GROUP_FILE, "CSV文件操作", CSVFileExample.Start);
            AddBtn(GROUP_EXTEND, "DateTime扩展", DateTimeExtendExample.Start);
            AddBtn(GROUP_EXTEND, "Transform扩展", TransformExtendExample.Start);
            AddBtn(GROUP_FRAMEWORK, "线程同步器", ThreadSyncExample.Start);
            AddBtn(GROUP_UTILS, "文件操作实用工具", FileUtilityExample.Start);
            AddBtn(GROUP_UTILS, "字符串操作实用工具", StringUtilityExample.Start);
            AddBtn(GROUP_UTILS, "时间操作实用工具", TimeUtilityExample.Start);
            AddBtn(GROUP_UTILS, "加密实用工具", CryptoExample.Start);
            AddBtn(GROUP_FILE, "Json使用", JsonExample.Start);
            AddBtn(GROUP_UTILS, "数组操作实用工具", ArrayUtilityExample.Start);
            AddBtn(GROUP_FRAMEWORK, "Messager消息收发系统", MessagerExample.Start);
            AddBtn(GROUP_FRAMEWORK, "对象池使用", ObjectPoolExample.Start);
            AddBtn(GROUP_FRAMEWORK, "视图工厂", ViewFactoryExample.Start);
            AddBtn(GROUP_FRAMEWORK, "异形屏适配", ScreenSafeAreaExample.Start);

            AddBtn(GROUP_FRAMEWORK, "框架常量", FrameworkConstExample.Start);

            AddBtn(GROUP_FRAMEWORK, "CoroutineProxy协程代理", CoroutineProxyExample.Start);
            AddBtn(GROUP_FRAMEWORK, "CoroutineQueue序列化执行协程", CoroutinesQueueExample.Start);
            AddBtn(GROUP_FRAMEWORK, "Zero UI库", ZeroUIExample.Start);
            AddBtn(GROUP_FRAMEWORK, "Zero EventListener库", ZeroEventListenerExample.Start);
            AddBtn(GROUP_AUDIO, "音频控制", AudioDeviceExample.Start);
            AddBtn(GROUP_FRAMEWORK, "Zero 资源操作", ResMgrExample.Start);
            AddBtn(GROUP_FRAMEWORK, "Zero 资源更新", ResUpdateExample.Start);
            AddBtn(GROUP_FRAMEWORK, "HotFiles资源", HotFilesExample.Start);

            AddBtn(GROUP_FRAMEWORK, "高级定时器", TimerExample.Start);
            AddBtn(GROUP_FRAMEWORK, "高级计时器", ChronographExample.Start);
            AddBtn(GROUP_FILE, "Zip 压缩/解压", ZipExample.Start);
            AddBtn(GROUP_BITMAPFONT, "位图字体使用", BitmapFontExample.Start);

            AddBtn(GROUP_AI, "行为树", BehaviacExample.Start);
            AddBtn(GROUP_AI, "有限状态机", FiniteStateMachineExample.Start);

            AddBtn(GROUP_UNITY, "纹理显示", TexturesExample.Start);
            AddBtn(GROUP_UNITY, "常用数据", UnityConstsExample.Start);

            AddBtn(GROUP_VIDEO, "视频播放", VideoExample.Start);

            OnToggleShowTodoValueChanged(toggleShowTodo.isOn);            
        }

        void AddBtn(string group, string label, Action action)
        {
            if (false == _groupItemDic.ContainsKey(group))
            {
                var go = GameObject.Instantiate(buttonGroupItem, content);
                go.name = $"MenuButtonGroupItem[{group}]";
                go.SetActive(true);
                _groupItemDic[group] = CreateChildView<MenuButtonGroupItem>(go, group);
            }
            var groupItem = _groupItemDic[group];
            groupItem.AddBtn(label, action);
        }

        private void OnToggleShowTodoValueChanged(bool isOn)
        {
            foreach (var item in _groupItemDic)
            {
                item.Value.SwitchButtonShow(isOn);
            }

            //以下代码为了强制刷新Layout组件
            this.gameObject.SetActive(false);
            //使用ILBridge来启动协程，避免因为GameObject的Active状态为false时，自身的协程不执行的问题
            ILBridge.Ins.StartCoroutine(DelayRefresh(this.gameObject));
        }

        IEnumerator DelayRefresh(GameObject obj)
        {
            yield return new WaitForEndOfFrame();
            obj.SetActive(true);
        }
    }
}


