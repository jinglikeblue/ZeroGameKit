using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZeroEditor
{
    public static class AssetBundleUtility
    {
        /// <summary>
        /// 创建AssetBundle依赖信息的JSON
        /// </summary>
        public static void CreateDependenciesJson(AssetBundleManifest manifest, String savePath)
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
    }
}
