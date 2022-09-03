using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    internal class ScriptsInitiator : BaseInitiator
    {
        internal override void Start()
        {
            base.Start();

            bool isUseDll = Runtime.Ins.IsUseDll;
            if (Runtime.Ins.BuiltinResMode == EBuiltinResMode.ONLY_USE)
            {
                //只使用内嵌资源的情况下，不需要加载DLL，直接执行打包的代码即可
                isUseDll = false;
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
            Debug.Log(Log.Zero1("@Scripts代码运行环境: [外部程序集]"));

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
            Debug.Log(Log.Zero1("@Scripts代码运行环境: [本地程序集]"));

            //初始化IL
            ILBridge.Ins.Startup();
        }



        byte[] LoadDllBytes()
        {
            string dllPath = FileUtility.CombinePaths( Runtime.Ins.localResDir, ZeroConst.DLL_DIR_NAME, ZeroConst.DLL_FILE_NAME + ".dll");
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
