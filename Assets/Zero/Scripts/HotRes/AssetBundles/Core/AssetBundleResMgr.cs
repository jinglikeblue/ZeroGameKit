using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zero
{
    public class AssetBundleResMgr : AResMgr
    {
        /// <summary>
        /// 资源描述
        /// </summary>
        AssetBundleManifest _manifest;

        /// <summary>
        /// 已加载的AB资源字典
        /// </summary>
        Dictionary<string, AssetBundle> _loadedABDic;

        /// <summary>
        /// 热更资源的AB文件根目录
        /// </summary>
        public string HotResAssetBundleRoot { get; private set; }

        /// <summary>
        /// 内嵌资源的AB文件根目录
        /// </summary>
        public string BuiltinAssetBundleRoot { get; private set; }

        public AssetBundleResMgr(string manifestFileName)
        {
            UnloadAll();
            _loadedABDic = new Dictionary<string, AssetBundle>();

            HotResAssetBundleRoot = FileUtility.CombinePaths(Runtime.Ins.localResDir, ZeroConst.AB_DIR_NAME);
            BuiltinAssetBundleRoot = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, ZeroConst.AB_DIR_NAME);

            //优先使用热更的
            var manifestPath = FileUtility.CombinePaths(HotResAssetBundleRoot, manifestFileName);
            if (false == File.Exists(manifestPath) || Runtime.Ins.IsOnlyUseBuiltinRes)
            {
                //使用内嵌的
                manifestPath = FileUtility.CombinePaths(BuiltinAssetBundleRoot, manifestFileName);
            }

            AssetBundle ab = AssetBundle.LoadFromFile(manifestPath);
            if (null == ab)
            {
                throw new Exception($"[{manifestFileName}] 不存在: {manifestPath}");
            }
            _manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            //{
            //    var dependenciesTable = new Dictionary<string, string[]>();
            //    var assetBundles = _manifest.GetAllAssetBundles();
            //    foreach (var assetBundle in assetBundles)
            //    {
            //        var dependencies = _manifest.GetAllDependencies(assetBundle);
            //        dependenciesTable[assetBundle] = dependencies;
            //    }
                
            //    var json = LitJson.JsonMapper.ToPrettyJson(dependenciesTable);
            //    Debug.Log(json);
            //}

            if (_manifest == null)
            {
                throw new Exception(string.Format("错误的 Manifest File: {0}", manifestFileName));
            }
            ab.Unload(false);
        }

        /// <summary>
        /// 让已加载的AB资源字典继承源资源管理器
        /// </summary>
        /// <param name="source"></param>
        internal void Inherit(AssetBundleResMgr source)
        {
            _loadedABDic = source._loadedABDic;
        }

        /// <summary>
        /// 如果
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        void MakeABNameNotEmpty(ref string abName)
        {
            if (string.IsNullOrEmpty(abName))
            {
                abName = ZeroConst.ROOT_AB_FILE_NAME;
            }
        }

        public override string[] GetDepends(string abName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            string[] dependList = _manifest.GetAllDependencies(abName);
            return dependList;
        }

        public override string[] GetAllAsssetsNames(string abName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            string[] assetNames = ab.GetAllAssetNames();
            for (int i = 0; i < assetNames.Length; i++)
            {
                assetNames[i] = Path.GetFileName(assetNames[i]);
            }
            return assetNames;
        }

        public override UnityEngine.Object Load(string abName, string assetName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            if (null == ab)
            {
                Debug.LogErrorFormat("AB资源不存在  abName: {0}", abName);
                return null;
            }
            var asset = ab.LoadAsset(assetName);
            if (null == asset)
            {
                Debug.LogErrorFormat("获取的资源不存在： AssetBundle: {0}  Asset: {1}", abName, assetName);
            }
            return asset;
        }

        public override T Load<T>(string abName, string assetName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            T asset = ab.LoadAsset<T>(assetName);
            if (null == asset)
            {
                Debug.LogErrorFormat("获取的资源不存在： AssetBundle: {0}  Asset: {1}", abName, assetName);
            }
            return asset;
        }

        public override UnityEngine.Object[] LoadAll(string abName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            return ab.LoadAllAssets();
        }

        public override void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            ILBridge.Ins.StartCoroutine(this, LoadAsync<UnityEngine.Object>(ab, assetName, onLoaded, onProgress));
        }

        public override void LoadAsync<T>(string abName, string assetName, Action<T> onLoaded, Action<float> onProgress = null)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            ILBridge.Ins.StartCoroutine(this, LoadAsync(ab, assetName, onLoaded, onProgress));
        }

        public override void LoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress = null)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            ILBridge.Ins.StartCoroutine(this, LoadAllAsync(ab, onLoaded, onProgress));
        }

        IEnumerator LoadAsync<T>(AssetBundle ab, string assetName, Action<T> onLoaded, Action<float> onProgress) where T : UnityEngine.Object
        {
            AssetBundleRequest abr = ab.LoadAssetAsync<T>(assetName);

            do
            {
                if (onProgress != null)
                {
                    onProgress.Invoke(abr.progress);
                }
                yield return new WaitForEndOfFrame();
            }
            while (false == abr.isDone);

            //加载完成
            onLoaded.Invoke((T)abr.asset);
        }


        IEnumerator LoadAllAsync(AssetBundle ab, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress)
        {
            AssetBundleRequest abr = ab.LoadAllAssetsAsync();

            do
            {
                if (onProgress != null)
                {
                    onProgress.Invoke(abr.progress);
                }
                yield return new WaitForEndOfFrame();
            }
            while (false == abr.isDone);

            //加载完成
            onLoaded.Invoke(abr.allAssets);
        }

        public override void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);
            if (_loadedABDic.ContainsKey(abName))
            {
                AssetBundle ab = _loadedABDic[abName];
                _loadedABDic.Remove(abName);
                ab.Unload(isUnloadAllLoaded);
                //Debug.LogFormat("释放AB：{0}", abName);

                if (isUnloadDepends)
                {
                    string[] dependList = _manifest.GetAllDependencies(abName);
                    foreach (string depend in dependList)
                    {
                        if (false == CheckDependencies(depend))
                        {
                            Unload(depend, isUnloadAllLoaded, isUnloadDepends);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 检查ab资源是否被已加载的资源依赖
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="depend"></param>
        /// <returns></returns>
        bool CheckDependencies(string ab)
        {
            foreach (var loadedEntry in _loadedABDic)
            {
                var entryDepends = _manifest.GetAllDependencies(loadedEntry.Key);
                foreach (var entryDepend in entryDepends)
                {
                    if (ab == entryDepend)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void UnloadAll(bool isUnloadAllLoaded = false)
        {
            if (null != _loadedABDic)
            {
                foreach (AssetBundle cached in _loadedABDic.Values)
                {
                    cached.Unload(isUnloadAllLoaded);
                }
                _loadedABDic.Clear();
            }

            ResMgr.Ins.DoGC();
        }

        AssetBundle LoadAssetBundleFromFile(string abName)
        {
            //优先使用热更的
            var abPath = FileUtility.CombinePaths(HotResAssetBundleRoot, abName);
            if (false == File.Exists(abPath) || Runtime.Ins.IsOnlyUseBuiltinRes)
            {
                //使用内嵌的
                abPath = FileUtility.CombinePaths(BuiltinAssetBundleRoot, abName);
            }

            if (ZeroLogSettings.ASSET_BUNDLE_LOAD_LOG_ENABLE)
            {
                Debug.Log($"[AssetBundle] 加载AssetBundle:{abPath}");
            }

            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            if (null == ab)
            {
                Debug.LogErrorFormat($"[AssetBundle] 文件 [{abName}] 不存在: {abPath}");
            }
            return ab;
        }

        /// <summary>
        /// 加载AB包，自动处理依赖问题
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string abName)
        {
            MakeABNameNotEmpty(ref abName);
            abName = ABNameWithExtension(abName);

            //依赖检查
            string[] dependList = _manifest.GetAllDependencies(abName);
            foreach (string depend in dependList)
            {
                //string dependPath = Path.Combine(_rootDir, depend);
                if (false == _loadedABDic.ContainsKey(depend))
                {
                    _loadedABDic[depend] = LoadAssetBundle(depend);
                }
            }

            AssetBundle ab = null;
            if (_loadedABDic.ContainsKey(abName))
            {
                ab = _loadedABDic[abName];
            }
            else
            {
                ab = LoadAssetBundleFromFile(abName);
                if (null == ab)
                {
                    return null;
                }
                _loadedABDic[abName] = ab;
            }

            return ab;
        }

    }
}
