﻿using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public static class HotResUtility
    {
        /// <summary>
        /// 创建AssetBundle依赖信息的JSON
        /// </summary>
        public static void CreateAssetBundleDependenciesJson(AssetBundleManifest manifest, String savePath)
        {
            var dependenciesTable = new Dictionary<string, string[]>();
            var assetBundles = manifest.GetAllAssetBundles();
            foreach (var ab in assetBundles)
            {
                var dependencies = manifest.GetAllDependencies(ab);
                dependenciesTable[ab] = dependencies;
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dependenciesTable, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(savePath, json);
        }

        /// <summary>
        /// 创建热更代码程序集
        /// </summary>
        public static void GeneateScriptAssembly()
        {
            var now = DateTime.Now;
            var cmd = new DllBuildCommand(ZeroEditorConst.HOT_SCRIPT_ROOT_DIR, ZeroEditorConst.DLL_PUBLISH_DIR);
            cmd.onFinished += (DllBuildCommand self, bool isSuccess) =>
            {
                var tip = isSuccess ? "Dll生成成功!" : "Dll生成失败!";
                Debug.Log(LogColor.Zero1(tip));
                Debug.Log(LogColor.Zero1("耗时:{0}秒", (DateTime.Now - now).TotalSeconds));
            };
            cmd.Execute();
        }
    }
}