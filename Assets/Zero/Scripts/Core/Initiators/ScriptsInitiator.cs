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

            var cfg = Runtime.Ins.VO;

            bool isUseDll = cfg.isUseDll;
            if (cfg.builtinResMode == EBuiltinResMode.ONLY_USE)
            {
                //只使用内嵌资源的情况下，不需要加载DLL，直接执行打包的代码即可
                isUseDll = false;
            }

            if (isUseDll)
            {
                RunWithDll();
            }
            else
            {
                RunWithoutDll();
            }

            End();
        }

        void RunWithoutDll()
        {
            Debug.Log(Log.Zero1("@Scripts代码运行环境: [本地程序集]"));

            //初始化IL
            ILBridge.Ins.Startup();
            //调用启动方法
            ILBridge.Ins.Invoke(Runtime.Ins.VO.className, Runtime.Ins.VO.methodName);
        }

        void RunWithDll()
        {
            Debug.Log(Log.Zero1("@Scripts代码运行环境: [外部程序集]"));

            var dllBytes = LoadDllBytes();
            byte[] pdbBytes = null;
            if (Runtime.Ins.VO.isLoadPdb)
            {
                pdbBytes = LoadPdbBytes();
            }

            //初始化IL
            ILBridge.Ins.Startup(dllBytes, pdbBytes);
            //调用启动方法
            ILBridge.Ins.Invoke(Runtime.Ins.VO.className, Runtime.Ins.VO.methodName);
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
