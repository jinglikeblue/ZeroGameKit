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
        [InfoBox("当项目为非热更项目时，ResMgr只能请求Resources目录中的资源", InfoMessageType = InfoMessageType.Info)]
        [LabelText("项目是否使用热更")]
        [OnValueChanged("OnIsHotResProjectChanged")]
        public bool isHotResProject;

        [SuffixLabel("关闭日志打印可以提高执行效率")]        
        [LabelText("是否允许打印日志")]
        public bool isLogEnable;

        [Title("启动")]
        [LabelText("启动类(完全限定类名)"), DisplayAsString]
        public string className = ZeroConst.LOGIC_SCRIPT_STARTUP_CLASS_NAME;

        [LabelText("启动方法(静态)"), DisplayAsString]
        public string methodName = ZeroConst.LOGIC_SCRIPT_STARTUP_METHOD;

        [Title("热更配置")]
        [InfoBox("$HotResSource", InfoMessageType = InfoMessageType.None)]
        [LabelText("资源的来源"), ValueDropdown("HotResMode")]    
        [ShowIf("isHotResProject")]
        public EHotResMode hotResMode;
        
        [InfoBox("Zero会按照队列依次尝试资源的下载，直到其中一个成功为止", InfoMessageType = InfoMessageType.Info)]
        [LabelText("网络资源的根目录"), ShowIf("hotResMode", EHotResMode.NET_ASSET_BUNDLE)]        
        public string[] netRoots = new string[1];

        [Title("热更代码配置")]
        [LabelText("使用DLL")]   
        [ShowIf("isHotResProject")]
        public bool isUseDll;

        [LabelText("DLL执行方式"), ValueDropdown("ILType"), ShowIf("isUseDll"), OnValueChanged("OnILTypeValueChange")]        
        public EILType ilType = EILType.IL_RUNTIME;

        [LabelText("调试功能"), ShowIf("ilType", EILType.IL_RUNTIME), ShowIf("isUseDll")]        
        public bool isDebugIL;

        [LabelText("加载Pdb文件"), ShowIf("isUseDll")]        
        public bool isLoadPdb;

#if UNITY_EDITOR
        IEnumerable HotResMode = new ValueDropdownList<EHotResMode>()
        {
            { "从网络资源目录加载资源（Release版采用）", EHotResMode.NET_ASSET_BUNDLE },
            { "从本地资源目录加载资源（资源上传前，本地测试AB包采用）", EHotResMode.LOCAL_ASSET_BUNDLE },
            { "使用AssetDataBase加载资源（限开发阶段使用）", EHotResMode.ASSET_DATA_BASE },
        };

        void OnIsHotResProjectChanged()
        {
            if (!isHotResProject)
            {
                hotResMode = EHotResMode.RESOURCES;
                isUseDll = false;
            }
        }

        string HotResSource()
        {
            string source = "资源来源:    {0}";
            switch (hotResMode)
            {
                case EHotResMode.NET_ASSET_BUNDLE:
                    source = string.Format(source, Log.Zero2("网络资源根目录"));
                    break;
                case EHotResMode.LOCAL_ASSET_BUNDLE:
                    source = string.Format(source, Log.Zero2(ZeroConst.PUBLISH_RES_ROOT_DIR));
                    break;
                case EHotResMode.ASSET_DATA_BASE:
                    source = string.Format(source, Log.Zero2(ZeroConst.HOT_RESOURCES_ROOT_DIR));
                    break;
                case EHotResMode.RESOURCES:
                    source = "从Resources中直接获取";
                    break;
            }
            return source;
        }

        void OnILTypeValueChange()
        {            
            PrefabEditNotice.Ins.onILTypeChanged?.Invoke(ilType);
        }

        IEnumerable ILType = new ValueDropdownList<EILType>()
        {
            { "JIT(平台不支持，会自动换为ILRuntime)", EILType.JIT },
            { "ILRuntime", EILType.IL_RUNTIME },
            { "HuaTuo", EILType.HUA_TUO },                 
        };
#endif
    }
}