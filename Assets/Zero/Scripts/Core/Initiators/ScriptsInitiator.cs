using Jing;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    internal class ScriptsInitiator : BaseInitiator
    {
        /// <summary>
        /// 热更DLL路径
        /// </summary>
        private string HotDllPath => FileUtility.CombinePaths(Runtime.localResDir, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");

        /// <summary>
        /// 是否存在热更DLL
        /// </summary>
        bool IsHotDllExist => File.Exists(HotDllPath);

        /// <summary>
        /// 是否存在内嵌DLL
        /// </summary>
        bool IsBuiltinDllExist => Runtime.BuiltinInitiator.IsBuiltinDllExist;

        /// <summary>
        /// 是否存在DLL
        /// </summary>
        bool IsDllExist => IsHotDllExist || IsBuiltinDllExist;

        internal override async UniTask<string> StartAsync(InitiatorProgress onProgress = null)
        {
            bool isUseDll = Runtime.IsUseDll;

            if (isUseDll && false == IsDllExist)
            {
                //开启了使用DLL，但是并没有dll存在
                if (Runtime.IsOfflineEnable)
                {
                    Debug.Log(Zero.LogColor.Zero1("[Zero][ScriptsInitiator] 没有找到dll文件，将自动用native脚本，以离线模式运行"));
                    isUseDll = false;
                }
                else
                {
                    return "[Zero][ScriptsInitiator] dll加载出错： 文件不存在";
                }
            }

            if (Runtime.IsUseDll != isUseDll)
            {
                Runtime.LauncherData.isUseDll = isUseDll;
            }

            string error = null;

            if (isUseDll)
            {
                error = StartupWithDll();
            }
            else
            {
                StartupWithoutDll();
            }

            if (null == error)
            {
                //调用启动方法
                ILBridge.Ins.Invoke(ZeroConst.LOGIC_SCRIPT_STARTUP_CLASS_NAME, ZeroConst.LOGIC_SCRIPT_STARTUP_METHOD);
            }

            return error;
        }

        string StartupWithDll()
        {
            Debug.Log(LogColor.Zero1("[Zero][ScriptsInitiator] 代码运行环境: [外部程序集(dll)]"));

            LoadDllBytes(out var dllBytes, out var pdbBytes);

            if (null == dllBytes)
            {
                return $"[Zero][ScriptsInitiator] dll启动失败！";
            }

            //尝试解密dll。如果dll本身是未加密状态，则会正常返回
            dllBytes = DllCryptoHelper.TryDecrypt(dllBytes);

            //初始化IL
            ILBridge.Ins.Startup(dllBytes, pdbBytes);
            return null;
        }

        void StartupWithoutDll()
        {
            Debug.Log(LogColor.Zero1("[Zero][ScriptsInitiator] 代码运行环境: [本地程序集]"));

            //初始化IL
            ILBridge.Ins.Startup();
        }

        void LoadDllBytes(out byte[] dllBytes, out byte[] pdbBytes)
        {
            dllBytes = null;
            pdbBytes = null;

            if (Runtime.IsHotResEnable)
            {
                string dllPath = HotDllPath;
                if (File.Exists(dllPath))
                {
                    dllBytes = File.ReadAllBytes(dllPath);
                    string pdbPath = FileUtility.CombinePaths(Runtime.localResDir, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".pdb");
                    if (File.Exists(pdbPath))
                    {
                        pdbBytes = File.ReadAllBytes(pdbPath);
                    }

                    return;
                }
            }

            if (IsBuiltinDllExist)
            {
                dllBytes = Runtime.BuiltinInitiator.DllBytes;
                pdbBytes = Runtime.BuiltinInitiator.PdbBytes;
            }
        }
    }
}