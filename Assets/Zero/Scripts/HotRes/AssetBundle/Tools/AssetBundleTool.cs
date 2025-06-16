using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    internal class AssetBundleTool : BaseAssetTool
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

        public AssetBundleTool(string manifestFileName)
        {
            InitVariables();

            //优先使用热更的
            var manifestPath = FileUtility.CombinePaths(HotResAssetBundleRoot, manifestFileName);
            if (false == File.Exists(manifestPath) || Runtime.IsOnlyUseBuiltinRes)
            {
                //使用内嵌的
                manifestPath = FileUtility.CombinePaths(BuiltinAssetBundleRoot, manifestFileName);
            }

            AssetBundle ab = AssetBundle.LoadFromFile(manifestPath);
            if (null == ab)
            {
                throw new Exception($"AssetBundleTool] [{manifestFileName}] 不存在: {manifestPath}");
            }

            InitManifest(ab);
        }

        public AssetBundleTool(AssetBundle manifest)
        {
            if (null == manifest)
            {
                throw new Exception($"[AssetBundleTool] manifest文件不存在!");
            }

            InitVariables();
            InitManifest(manifest);
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        void InitVariables()
        {
            UnloadAll();
            _loadedABDic = new Dictionary<string, AssetBundle>();
            HotResAssetBundleRoot = FileUtility.CombinePaths(Runtime.localResDir, ZeroConst.AB_DIR_NAME);
            BuiltinAssetBundleRoot = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, ZeroConst.AB_DIR_NAME);
        }

        void InitManifest(AssetBundle ab)
        {
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

            if (!_manifest)
            {
                throw new Exception($"错误的 Manifest File: {ab.name}");
            }

            ab.Unload(false);
        }

        /// <summary>
        /// 让已加载的AB资源字典继承源资源管理器
        /// </summary>
        /// <param name="source"></param>
        internal void Inherit(AssetBundleTool source)
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
            try
            {
                AssetBundle ab = LoadAssetBundle(abName);
                T asset = ab.LoadAsset<T>(assetName);
                return asset;
            }
            catch (Exception e)
            {
                var assetPath = Assets.GetOriginalAssetPath(abName, assetName);
                Debug.Log($"获取的资源不存在： AssetBundle: {abName}  Asset: {assetName}  AssetPath: {assetPath}");
                return null;
            }
        }

        public override UnityEngine.Object[] LoadAll(string abName)
        {
            AssetBundle ab = LoadAssetBundle(abName);
            if (ab.isStreamedSceneAssetBundle)
            {
                return Array.Empty<UnityEngine.Object>();
            }

            return ab.LoadAllAssets();
        }

        public override AssetBundle TryLoadAssetBundle(string abName)
        {
            // MakeABNameNotEmpty(ref abName);
            // abName = ABNameWithExtension(abName);
            AssetBundle ab = LoadAssetBundle(abName);
            return ab;
        }

        public override async UniTask<AssetBundle> TryLoadAssetBundleAsync(string abName, Action<float> onProgress = null)
        {
            var ab = await LoadAssetBundleAsync(abName, onProgress);
            return ab;
        }

        public override async void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            AssetBundle ab = await LoadAssetBundleAsync(abName, onProgress);
            LoadAsync<UnityEngine.Object>(ab, assetName, onLoaded, onProgress);
        }

        public override async void LoadAsync<T>(string abName, string assetName, Action<T> onLoaded,
            Action<float> onProgress = null)
        {
            AssetBundle ab = await LoadAssetBundleAsync(abName, onProgress);
            LoadAsync(ab, assetName, onLoaded, onProgress);
        }

        public override async void LoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded,
            Action<float> onProgress = null)
        {
            AssetBundle ab = await LoadAssetBundleAsync(abName, onProgress);
            LoadAllAsync(ab, onLoaded, onProgress);
        }

        async void LoadAsync<T>(AssetBundle ab, string assetName, Action<T> onLoaded, Action<float> onProgress) where T : UnityEngine.Object
        {
            try
            {
                AssetBundleRequest abr = ab.LoadAssetAsync<T>(assetName);

                do
                {
                    if (onProgress != null)
                    {
                        onProgress.Invoke(abr.progress);
                    }

                    await UniTask.NextFrame();
                } while (false == abr.isDone);

                //加载完成
                onLoaded.Invoke((T)abr.asset);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                onLoaded?.Invoke(null);
            }
        }

        // IEnumerator LoadAsync<T>(AssetBundle ab, string assetName, Action<T> onLoaded, Action<float> onProgress)
        //     where T : UnityEngine.Object
        // {
        //     AssetBundleRequest abr = ab.LoadAssetAsync<T>(assetName);
        //
        //     do
        //     {
        //         if (onProgress != null)
        //         {
        //             onProgress.Invoke(abr.progress);
        //         }
        //
        //         yield return new WaitForEndOfFrame();
        //     } while (false == abr.isDone);
        //
        //     //加载完成
        //     onLoaded.Invoke((T)abr.asset);
        // }


        async void LoadAllAsync(AssetBundle ab, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress)
        {
            AssetBundleRequest abr = ab.LoadAllAssetsAsync();

            do
            {
                if (onProgress != null)
                {
                    onProgress.Invoke(abr.progress);
                }

                await UniTask.NextFrame();
            } while (false == abr.isDone);

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

            Runtime.GC();
        }

        AssetBundle LoadAssetBundleFromFile(string abName)
        {
            string abPath;
            if (WebGL.IsEnvironmentWebGL)
            {
                abPath = WebGL.MakeAbsolutePath(abName, EResType.Asset);
            }
            else
            {
                //优先使用热更的
                abPath = FileUtility.CombinePaths(HotResAssetBundleRoot, abName);
                if (false == File.Exists(abPath) || Runtime.IsOnlyUseBuiltinRes)
                {
                    //使用内嵌的
                    abPath = FileUtility.CombinePaths(BuiltinAssetBundleRoot, abName);
                }
            }

            if (ZeroLogSettings.ASSET_BUNDLE_LOAD_LOG_ENABLE)
            {
                Debug.Log($"[AssetBundle] 加载AssetBundle:{abPath}");
            }

            AssetBundle ab;

            if (WebGL.IsEnvironmentWebGL)
            {
                ab = WebGL.GetAssetBundle(abName, true);
            }
            else
            {
                ab = AssetBundle.LoadFromFile(abPath);
            }

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
            abName = abName.ToLower();
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

        /// <summary>
        /// 异步加载AB包，自动处理依赖问题
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="onProgress">如果资源存在预载的情况，该委托会更新进度信息</param>
        /// <returns></returns>
        private async UniTask<AssetBundle> LoadAssetBundleAsync(string abName, Action<float> onProgress)
        {
            //预载资源
            await Res.Prepare(new string[] { abName }, info =>
            {
                onProgress?.Invoke(info.Progress);
            });

            return LoadAssetBundle(abName);
        }
    }
}