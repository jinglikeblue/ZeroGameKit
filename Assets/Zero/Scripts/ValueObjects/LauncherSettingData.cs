using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 运行时数据对象
    /// </summary>
    [Serializable]
    [HideLabel]
    public class LauncherSettingData
    {
        [Title("基础")]                               
        [SuffixLabel("关闭日志打印可以提高执行效率")]        
        [LabelText("是否允许打印日志")]
        [OnValueChanged("OnValueChanged")]
        public bool isLogEnable = true;

        // [ShowInInspector]
        // [Title("启动")]
        // [LabelText("启动类(完全限定类名)"), DisplayAsString]
        // protected string className = ZeroConst.LOGIC_SCRIPT_STARTUP_CLASS_NAME;
        //
        // [ShowInInspector]
        // [LabelText("启动方法(静态)"), DisplayAsString]
        // protected string methodName = ZeroConst.LOGIC_SCRIPT_STARTUP_METHOD;

        [Title("资源加载")] 
        [InfoBox("「启用」ResMgr通过AssetBundle相关接口加载资源。", VisibleIf = "isUseAssetBundle")]
        [InfoBox("「关闭」ResMgr通过AssetDataBase接口加载资源。", VisibleIf = "@!isUseAssetBundle")]
        [InfoBox("<color=#D6E884>非Editor环境时，会强制使用AssetBundle模式。</color>", InfoMessageType.None)]
        [OnValueChanged("OnValueChanged")]
        [LabelText("AssetBundle模式")]
        public bool isUseAssetBundle = false;
        
        [Title("热更")]
        [InfoBox("「启用」AssetBundle模式下运行时，会自动检测网络，下载最新的网络资源。网络资源的根目录可以配置多个。框架会自动按照顺序尝试使用。", VisibleIf = "isHotPatchEnable")]
        [InfoBox("「关闭」仅使用[StreamingAssets/res]下的资源。", VisibleIf = "@!isHotPatchEnable")]
        [LabelText("热更功能")]
        [OnValueChanged("OnValueChanged")]
        [ShowIf("isUseAssetBundle")]
        public bool isHotPatchEnable = false;

        [LabelText("网络资源的根目录(示例: http://YourHotResRootUrl)")]
        [ListDrawerSettings(ShowFoldout = false, NumberOfItemsPerPage = 3, DefaultExpandedState = true)]            
        [ShowIf("@isHotPatchEnable&&isUseAssetBundle")]
        [OnValueChanged("OnValueChanged")]  
        public string[] urlRoots = new string[1] { "http://YourHotResRootUrl" };
        
        [LabelText("允许离线运行")]
        [ShowIf("@isUseAssetBundle&&isHotPatchEnable")]
        [OnValueChanged("OnValueChanged")]
        [InfoBox("开启后。无论资源是否能更新，都可以用现有资源运行程序。", VisibleIf = "isOfflineEnable")]
        [HideInInspector] //隐藏功能，开放设置可能导致新手难以理解。
        public bool isOfflineEnable = false;

        [Title("代码")]
        [InfoBox("通过dll文件运行程序。开启后会自动检测HybridCLR安装状态。", VisibleIf = "isUseDll")]
        [SuffixLabel("热更程序集主类：Zero.Main")]
        // [InfoBox("不使用dll的情况下，Build时会自动关闭HybridCLR功能。", VisibleIf = "@!isUseDll")]
        [LabelText("使用dll")]
        [OnValueChanged("OnValueChanged")]
        public bool isUseDll = false;

#if UNITY_EDITOR

        public event Action onChange;

        void OnValueChanged()
        {
            onChange?.Invoke();            
        }
#endif
    }
}