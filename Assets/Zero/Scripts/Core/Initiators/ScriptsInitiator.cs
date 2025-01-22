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
        bool IsBuiltinDllExist => Runtime.Ins.streamingAssetsResInitiator.IsResExist;
        
        /// <summary>
        /// 是否存在DLL
        /// </summary>
        bool IsDllExist => IsHotDllExist || IsBuiltinDllExist;
        
        internal override void Start()
        {
            base.Start();

            bool isUseDll = Runtime.Ins.IsUseDll;
            if (Runtime.Ins.BuiltinResMode == EBuiltinResMode.ONLY_USE)
            {
                //只使用内嵌资源的情况下，不需要加载DLL，直接执行打包的代码即可
                isUseDll = false;
            }

            #region 如果是调试模式，并且存在DLL，那么就使用DLL

            if (false == isUseDll)
            {
                if (Runtime.Ins.IsDebugDll && IsDllExist)
                {
                    Debug.Log(LogColor.Zero1("进入Dll调试模式"));
                    isUseDll = true;
                }
            }

            #endregion

            
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

            var dllBytes = LoadDllBytes();
            byte[] pdbBytes = null;
            if (Runtime.Ins.IsLoadPdb)
            {
                pdbBytes = LoadPdbBytes();
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
        
        byte[] LoadDllBytes()
        {
            string dllPath = HotDllPath;
            if (File.Exists(dllPath))
            {
                return File.ReadAllBytes(dllPath);
            }

            if (Runtime.Ins.streamingAssetsResInitiator.IsResExist)
            {
                return Runtime.Ins.streamingAssetsResInitiator.scriptDllBytes;
            }


            throw new Exception($"DLL加载出错： {dllPath}");
        }

        byte[] LoadPdbBytes()
        {
            string pdbPath = FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".pdb");
            if (File.Exists(pdbPath))
            {
                return File.ReadAllBytes(pdbPath);
            }

            if (Runtime.Ins.streamingAssetsResInitiator.IsResExist)
            {
                return Runtime.Ins.streamingAssetsResInitiator.scriptPdbBytes;
            }

            throw new Exception($"PDB加载出错： {pdbPath}");
        }
    }
}