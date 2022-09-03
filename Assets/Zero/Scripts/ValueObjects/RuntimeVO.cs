using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 运行时数据对象
    /// </summary>
    [Serializable]
    [HideLabel]
    public class RuntimeVO
    {
        [Title("基础")]                               
        [SuffixLabel("关闭日志打印可以提高执行效率")]        
        [LabelText("是否允许打印日志")]
        [OnValueChanged("OnValueChanged")]
        public bool isLogEnable;

        [ShowInInspector]
        [Title("启动")]
        [LabelText("启动类(完全限定类名)"), DisplayAsString]
        protected string className = ZeroConst.LOGIC_SCRIPT_STARTUP_CLASS_NAME;

        [ShowInInspector]
        [LabelText("启动方法(静态)"), DisplayAsString]
        protected string methodName = ZeroConst.LOGIC_SCRIPT_STARTUP_METHOD;

        [Title("内嵌资源配置")]
        [InfoBox("如果希望所有资源都从网络下载，删除[StreamingAssets/res]目录即可\r\n也可以根据需求删除部分内嵌资源，只需要重新对内嵌资源目录生成res.json即可", VisibleIf = "$IsHotPatchBuiltinResMode")]
        [InfoBox("$BuiltinResSource", InfoMessageType = InfoMessageType.None)]
        [LabelText("模式"), ValueDropdown("BuiltinResMode")]                
        [OnValueChanged("OnValueChanged")]
        public EBuiltinResMode builtinResMode;

        [Title("资源读取配置")]        
        [InfoBox("$HotResSource", InfoMessageType = InfoMessageType.None)]
        [LabelText("资源读取模式"), ValueDropdown("HotResMode")]
        [OnValueChanged("OnValueChanged")]
        public EHotResMode hotResMode;
        
        [InfoBox("Zero会按照队列依次尝试资源的下载，直到其中一个成功为止", InfoMessageType = InfoMessageType.Info)]
        [LabelText("网络资源的根目录"), ShowIf("hotResMode", EHotResMode.NET_ASSET_BUNDLE)]
        [OnValueChanged("OnValueChanged")]
        public string[] netRoots = new string[1];

        [InfoBox("仅使用内嵌资源模式下，将不会使用DLL运行项目，代码将直接运行以达到代码执行效率最佳化。", InfoMessageType.Warning, VisibleIf = "$IsOnlyUseBuiltinResMode")]
        [Title("热更代码配置")]
        [LabelText("使用dll")]
        [OnValueChanged("OnValueChanged")]
        public bool isUseDll;

        [LabelText("加载pdb"), ShowIf("isUseDll")]
        [OnValueChanged("OnValueChanged")]
        public bool isLoadPdb;

        [LabelText("DLL执行方式"), ValueDropdown("ILType"), ShowIf("isUseDll")]
        [OnValueChanged("OnValueChanged")]
        public EILType ilType = EILType.IL_RUNTIME;

        [LabelText("优先JIT"), SuffixLabel("JIT方式更高效，如果平台不支持则继续ILRuntime模式"), ShowIf("$IsShowILRuntimeDebug")]
        [OnValueChanged("OnValueChanged")]
        public bool isTryJitBeforeILRuntime;

        [LabelText("调试功能"), ShowIf("$IsShowILRuntimeDebug")]
        [OnValueChanged("OnValueChanged")]
        public bool isDebugIL;

#if UNITY_EDITOR

        public event Action onChange;

        void OnValueChanged()
        {            
            onChange?.Invoke();
        }

        bool IsOnlyUseBuiltinResMode()
        {
            return builtinResMode == EBuiltinResMode.ONLY_USE ? true : false;
        }

        bool IsHotPatchBuiltinResMode()
        {
            return builtinResMode == EBuiltinResMode.HOT_PATCH ? true : false;
        }

        IEnumerable BuiltinResMode = new ValueDropdownList<EBuiltinResMode>()
        {
            { "[推荐]热补丁模式(HOT_PATCH)", EBuiltinResMode.HOT_PATCH },            
            { "仅使用内嵌资源(ONLY_USE)", EBuiltinResMode.ONLY_USE },
        };        

        string BuiltinResSource()
        {
            string source = "资源来源:    {0}";
            switch (builtinResMode)
            {
                case EBuiltinResMode.HOT_PATCH:
                    source = $"{Log.Zero2("热补丁模式（推荐）。优先使用通过网络更新到的资源，其次使用内嵌的资源")}";
                    break;
                case EBuiltinResMode.ONLY_USE:
                    source = $"{Log.Zero2("该模式可以完全脱离网络运行，非常适合用来制作[不联网的单机游戏]。")}";
                    break;
            }
            return source;
        }


        IEnumerable HotResMode = new ValueDropdownList<EHotResMode>()
        {
            { "网络资源(NET_ASSET_BUNDLE)", EHotResMode.NET_ASSET_BUNDLE },
            { "[限开发]本地热更资源(LOCAL_ASSET_BUNDLE)", EHotResMode.LOCAL_ASSET_BUNDLE },
            { "[限开发]项目开发资源(ASSET_DATA_BASE)", EHotResMode.ASSET_DATA_BASE },
        };

        string HotResSource()
        {
            string source = "";
            switch (hotResMode)
            {
                case EHotResMode.NET_ASSET_BUNDLE:
                    source = $"[部署模式]资源来源:    {Log.Zero2("网络资源根目录")}。从网络资源目录加载资源（Release版采用）";
                    break;
                case EHotResMode.LOCAL_ASSET_BUNDLE:
                    source = $"[开发模式]资源来源:    {Log.Zero2(ZeroConst.PUBLISH_RES_ROOT_DIR)}。从本地资源目录加载资源（资源上传前，本地测试AB包采用）";
                    break;
                case EHotResMode.ASSET_DATA_BASE:
                    source = $"[开发模式]资源来源:    {Log.Zero2(ZeroConst.HOT_RESOURCES_ROOT_DIR)}。使用AssetDataBase加载资源（开发阶段使用）";
                    break;
            }
            return source;
        }

        IEnumerable ILType = new ValueDropdownList<EILType>()
        {            
            { "ILRuntime", EILType.IL_RUNTIME },
            { "HybridCLR", EILType.HYBRID_CLR },                 
        };

        bool IsShowILRuntimeDebug()
        {
            if(isUseDll && EILType.IL_RUNTIME == ilType)
            {
                return true;
            }
            return false;
        }
#endif
    }
}