using Jing;
using System;
using System.IO;
using UnityEngine;

namespace Zero
{
    internal class ScriptsInitiator : BaseInitiator
    {
        /// <summary>
        /// 热更DLL路径
        /// </summary>
        private string HotDllPath => FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");

        /// <summary>
        /// 是否存在热更DLL
        /// </summary>
        bool IsHotDllExist => File.Exists(HotDllPath);
        
        /// <summary>
        /// 是否存在内嵌DLL
        /// </summary>
        bool IsBuiltinDllExist => Runtime.Ins.streamingAssetsResInitiator.IsBuiltinDllExist;
        
        /// <summary>
        /// 是否存在DLL
        /// </summary>
        bool IsDllExist => IsHotDllExist || IsBuiltinDllExist;
        
        internal override void Start()
        {
            base.Start();

            bool isUseDll = Runtime.Ins.IsUseDll;

            if (isUseDll && false == IsDllExist)
            {
                //开启了使用DLL，但是并没有dll存在
                if (Runtime.Ins.IsOfflineEnable)
                {
                    Debug.Log(Zero.LogColor.Zero1("[Launcher] 没有找到dll文件，将自动用native脚本，以离线模式运行"));
                    isUseDll = false;
                }
                else
                {
                    throw new Exception($"[Launcher][StriptsInitiator] DLL加载出错： 文件不存在");
                }
            }
            
            if (isUseDll)
            {
                StartupWithDll();
            }
            else
            {
                StartupWithoutDll();
            }

            //调用启动方法
            ILBridge.Ins.Invoke(ZeroConst.LOGIC_SCRIPT_STARTUP_CLASS_NAME, ZeroConst.LOGIC_SCRIPT_STARTUP_METHOD);

            End();
        }

        void StartupWithDll()
        {
            Debug.Log(LogColor.Zero1("@Scripts代码运行环境: [外部程序集]"));

            LoadDllBytes(out var dllBytes, out var pdbBytes);

            if (null == dllBytes)
            {
                throw new Exception($"[@Scripts] dll启动失败！");
            }
            
            //初始化IL
            ILBridge.Ins.Startup(dllBytes, pdbBytes);
        }

        void StartupWithoutDll()
        {
            Debug.Log(LogColor.Zero1("@Scripts代码运行环境: [本地程序集]"));

            //初始化IL
            ILBridge.Ins.Startup();
        }

        void LoadDllBytes(out byte[] dllBytes, out byte[] pdbBytes)
        {
            dllBytes = null;
            pdbBytes = null;

            if (Runtime.Ins.VO.isHotPatchEnable)
            {
                string dllPath = HotDllPath;
                if (File.Exists(dllPath))
                {
                    dllBytes = File.ReadAllBytes(dllPath);
                    string pdbPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".pdb");
                    if (File.Exists(pdbPath))
                    {
                        pdbBytes = File.ReadAllBytes(pdbPath);
                    }
                    return;
                }
            }

            if (IsBuiltinDllExist)
            {
                dllBytes =  Runtime.Ins.streamingAssetsResInitiator.scriptDllBytes;
                pdbBytes = Runtime.Ins.streamingAssetsResInitiator.scriptPdbBytes;
            }
        }
    }
}