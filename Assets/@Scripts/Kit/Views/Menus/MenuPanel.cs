using Example;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroHot;

namespace ZeroGameKit
{
    public class MenuPanel : AView
    {
        const string GROUP_DEMO = "DEMO";
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

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            buttonGroupItem.SetActive(false);

            AddBtn(GROUP_DEMO, "2D物理游戏DEMO", RoushanExample.Start);
            AddBtn(GROUP_DEMO, "DEMO_推箱子", null);
            AddBtn(GROUP_DEMO, "3D游戏DEMO", null);

            AddBtn(GROUP_PERFORMANCE, "运算性能", PerformanceExample.Calculate);
            AddBtn(GROUP_PERFORMANCE, "与主工程交互性能", PerformanceExample.CallNative);

            AddBtn(GROUP_ANDROID, "Android交互", AndroidBridgeExample.Start);
            AddBtn(GROUP_FRAMEWORK, "资源获取", ResLoadExample.Start);
            AddBtn(GROUP_DOTWEEN, "DoTween", DoTweenExample.Start);
            AddBtn(GROUP_UNIWEBVIEW, "网页浏览", null);
            AddBtn(GROUP_FRAMEWORK, "单例使用示例", SingletonClassExample.Start);
            AddBtn(GROUP_FRAMEWORK, "消息窗口", MsgWinExample.Start);
            AddBtn(GROUP_FRAMEWORK, "配置使用示例", ConfigExample.Start);
            AddBtn(GROUP_TURBOCHARGESCROLLLIST, "高性能列表", null);
            AddBtn(GROUP_NET, "WebSocket使用", null);
            AddBtn(GROUP_NET, "Socket使用", null);
            AddBtn(GROUP_NET, "Web请求", null);
            AddBtn(GROUP_FILE, "Protobuf使用", ProtoBufExample.Start);
            AddBtn(GROUP_NET, "网络文件下载", null);            
            AddBtn(GROUP_FILE, "字节数组操作", ByteArrayExample.Start);
            AddBtn(GROUP_FILE, "CSV文件操作", null);
            AddBtn(GROUP_EXTEND, "DateTime扩展", DateTimeExtendExample.Start);
            AddBtn(GROUP_EXTEND, "Transform扩展", TransformExtendExample.Start);                        
            AddBtn(GROUP_FRAMEWORK, "线程同步器", ThreadSyncExample.Start);
            AddBtn(GROUP_UTILS, "文件操作实用工具", null);
            AddBtn(GROUP_UTILS, "MD5实用工具", MD5Example.Start);
            AddBtn(GROUP_UTILS, "字符串操作实用工具", null);
            AddBtn(GROUP_UTILS, "时间操作实用工具", null);
            AddBtn(GROUP_FILE, "Json使用", JsonExample.Start);
            AddBtn(GROUP_UTILS, "数组操作实用工具", ArrayUtilityExample.Start);
            AddBtn(GROUP_FRAMEWORK, "Messager消息收发系统", null);
            AddBtn(GROUP_FRAMEWORK, "对象池使用", null);
            AddBtn(GROUP_FRAMEWORK, "视图工厂", null);
            AddBtn(GROUP_FRAMEWORK, "自动化配置表", null);

            AddBtn(GROUP_FRAMEWORK, "框架常量", FrameworkConstExample.Show);

            AddBtn(GROUP_FRAMEWORK, "CoroutineProxy协程代理", null);
            AddBtn(GROUP_FRAMEWORK, "Zero UI库", null);
            AddBtn(GROUP_FRAMEWORK, "Zero EventListener库", null);
            AddBtn(GROUP_FRAMEWORK, "Zero Data库", null);
            AddBtn(GROUP_AUDIO, "音频控制", AudioDeviceExample.Start);            
            AddBtn(GROUP_FRAMEWORK, "Zero 资源操作", null);
            AddBtn(GROUP_FRAMEWORK, "Zero 资源更新", null);
            AddBtn(GROUP_FRAMEWORK, "加密实用工具", null);
            AddBtn(GROUP_FRAMEWORK, "高级计时器", null);
            AddBtn(GROUP_FILE, "Zip解压缩", null);
            AddBtn(GROUP_BITMAPFONT, "位图字体使用", BitmapFontExample.Start);

            AddBtn(GROUP_AI, "行为树", BehaviacExample.Start);
            AddBtn(GROUP_AI, "有限状态机", null);

            AddBtn(GROUP_UNITY, "纹理显示", TexturesExample.Start);

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
            foreach(var item in _groupItemDic)
            {
                item.Value.SwitchButtonShow(isOn);
            }

            //以下代码为了强制刷新Layout组件
            this.gameObject.SetActive(false);
            //使用ILBridge来启动协程，避免因为GameObject的Active状态为false时，自身的协程不执行的问题
            ILBridge.Ins.StartCoroutine(DelayRefresh()); 
        }

        IEnumerator DelayRefresh()
        {
            yield return new WaitForEndOfFrame();
            this.gameObject.SetActive(true);
        }
    }
}


