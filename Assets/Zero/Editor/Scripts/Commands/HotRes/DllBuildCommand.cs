﻿using Jing;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class DllBuildCommand
    {
        /// <summary>
        /// 打包完成，返回一个bool表示成功还是失败
        /// </summary>
        public event Action<DllBuildCommand, bool> onFinished;

        string _sourcesDir;

        string _outputDir;

        string _outputAssemblyPath;

        string _outputAssemblyCachePath;

        /// <summary>
        /// 是否加密Dll
        /// </summary>
        public bool IsEncryptDll = true;

        /// <summary>
        /// 代码地址
        /// </summary>
        public string assemblyPath
        {
            get { return _outputAssemblyPath; }
        }

        public DllBuildCommand(string sourcesDir, string outputDir)
        {
            _sourcesDir = sourcesDir;
            _outputDir = outputDir;
            if (false == Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            if (false == Directory.Exists(ZeroEditorConst.DLL_CACHE_DIR))
            {
                Directory.CreateDirectory(ZeroEditorConst.DLL_CACHE_DIR);
            }

            _outputAssemblyPath = FileUtility.CombinePaths(outputDir, ZeroConst.DLL_FILE_NAME + ".dll");
            _outputAssemblyCachePath = FileUtility.CombinePaths(ZeroEditorConst.DLL_CACHE_DIR, ZeroConst.DLL_FILE_NAME + ".dll");
        }

        public void Execute()
        {
            var scriptPaths = Directory.GetFiles(_sourcesDir, "*.cs", SearchOption.AllDirectories);
            var ab = new AssemblyBuilder(_outputAssemblyCachePath, scriptPaths);
            ab.compilerOptions = new ScriptCompilerOptions();
            //添加[-deterministic]参数，确保代码不变的情况下，生成的DLL文件是一致的
            ab.compilerOptions.AdditionalCompilerArguments = new[] { "-deterministic" };
#if UNITY_2019_1_OR_NEWER
            ab.referencesOptions = ReferencesOptions.UseEngineModules;
#endif
            ab.flags = AssemblyBuilderFlags.DevelopmentBuild | AssemblyBuilderFlags.EditorAssembly;
            ab.additionalReferences = GetDepends();
            ab.buildFinished += OnFinished;
            if (false == ab.Build())
            {
                onFinished?.Invoke(this, false);
                onFinished = null;
            }
        }

        string[] GetDepends()
        {
            //依赖Assets下的DLL
            var assetDir = Application.dataPath;
            var dllList0 = Directory.GetFiles(assetDir, "*.dll", SearchOption.AllDirectories);

            //Unity2019.4.37里面打包发现用不着这块DLL的引用
            dllList0 = new string[0];

            //依赖Library/ScriptAssemblies下的DLL
            var projectDir = Directory.GetParent(assetDir).FullName;
            var dllList1 = Directory.GetFiles(FileUtility.CombineDirs(true, projectDir, "Library", "ScriptAssemblies"), "*.dll", SearchOption.AllDirectories);

#if UNITY_2019_1_OR_NEWER
            var dllList2 = new string[0];
#else
            //依赖Unity安装目录下的DLL
            var dir = FileUtility.CombineDirs(true, EditorApplication.applicationContentsPath, "Managed", "UnityEngine");
            var dllList2 = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
#endif

            string[] depends = new string[dllList0.Length + dllList1.Length + dllList2.Length];
            Array.Copy(dllList0, 0, depends, 0, dllList0.Length);
            Array.Copy(dllList1, 0, depends, dllList0.Length, dllList1.Length);
            Array.Copy(dllList2, 0, depends, dllList0.Length + dllList1.Length, dllList2.Length);
            return depends;
        }

        private void OnFinished(string path, CompilerMessage[] msgs)
        {
            bool isFail = false;
            foreach (var msg in msgs)
            {
                if (msg.type == CompilerMessageType.Error)
                {
                    Debug.LogError(msg.message);
                    isFail = true;
                }
            }

            if (isFail)
            {
                onFinished?.Invoke(this, false);
            }
            else
            {
                if (IsEncryptDll)
                {
                    Debug.Log($"加密dll");
                    var bytes = File.ReadAllBytes(_outputAssemblyCachePath);
                    var encryptedDllBytes = DllCryptoHelper.Encrypt(bytes);
                    File.WriteAllBytes(_outputAssemblyPath, encryptedDllBytes);
                }
                else
                {
                    //把缓存目录的dll文件和pdb文件复制到发布目录
                    FileUtility.CopyFile(_outputAssemblyCachePath, _outputAssemblyPath, true);
                }

                FileUtility.CopyFile(_outputAssemblyCachePath.Replace(".dll", ".pdb"), _outputAssemblyPath.Replace(".dll", ".pdb"), true);
                onFinished?.Invoke(this, true);
            }

            onFinished = null;
        }
    }
}