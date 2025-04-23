using Jing;
using System;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResMgr
    {
        public enum EResMgrType
        {
            ASSET_BUNDLE,
            RESOURCES,
            ASSET_DATA_BASE,
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static ResMgr Ins { get; } = new ResMgr();

        private ResMgr()
        {
        }

        AResMgr _mgr;

        public void Init(EResMgrType type, string assetsInfo = null)
        {
            switch (type)
            {
                case EResMgrType.ASSET_BUNDLE:
                    Debug.Log(LogColor.Zero1("初始化资源管理器... 资源来源：[AssetBundle]  Manifest名称：{0}", assetsInfo));
                    var newMgr = new AssetBundleResMgr(assetsInfo);
                    if (_mgr != null && _mgr is AssetBundleResMgr)
                    {
                        //替换旧的需要继承一下已加载字典
                        newMgr.Inherit(_mgr as AssetBundleResMgr);
                    }

                    _mgr = newMgr;
                    break;
                case EResMgrType.RESOURCES:
                    Debug.Log(LogColor.Zero1("初始化资源管理器... 资源来源：[Resources]"));
                    _mgr = new ResourcesResMgr();
                    break;
                case EResMgrType.ASSET_DATA_BASE:
                    Debug.Log(LogColor.Zero1("初始化资源管理器... 资源来源：[AssetDataBase] 资源根目录：{0}", assetsInfo));
                    _mgr = new AssetDataBaseResMgr(assetsInfo);
                    break;
            }
        }

        /// <summary>
        /// 执行一次内存回收(该接口开销大，可能引起卡顿)
        /// </summary>
        public void DoGC()
        {
            //移除没有引用的资源
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 得到AB资源的依赖
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public string[] GetDepends(string abName)
        {
            return _mgr.GetDepends(abName);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abName">资源包名称</param>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        /// <param name="isUnloadDepends">是否卸载关联的资源</param>
        public void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            _mgr.Unload(abName, isUnloadAllLoaded, isUnloadDepends);
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        public void UnloadAll(bool isUnloadAllLoaded = false)
        {
            _mgr.UnloadAll(isUnloadAllLoaded);
        }

        /// <summary>
        /// 获取AB中所有资源的名称
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public string[] GetAllAsssetsNames(string abName)
        {
            return _mgr.GetAllAsssetsNames(abName);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public UnityEngine.Object Load(string abName, string assetName)
        {
            return _mgr.Load(abName, assetName);
        }

        /// <summary>
        /// 获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public UnityEngine.Object[] LoadAll(string abName)
        {
            return _mgr.LoadAll(abName);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public UnityEngine.Object Load(string assetPath)
        {
            string abName;
            string assetName;
            SeparateAssetPath(assetPath, out abName, out assetName);
            return Load(abName, assetName);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public T Load<T>(string abName, string assetName) where T : UnityEngine.Object
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            T obj = _mgr.Load<T>(abName, assetName);
            //sw.Stop();
            //Debug.LogFormat("获取资源耗时: {0}", sw.Elapsed.TotalMilliseconds);

            return obj;
        }

        /// <summary>
        /// 通过资源路径加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源的路径</param>
        /// <returns></returns>
        public T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            string abName;
            string assetName;
            SeparateAssetPath(assetPath, out abName, out assetName);
            return Load<T>(abName, assetName);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            _mgr.LoadAsync(abName, assetName, onLoaded, onProgress);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync<T>(string abName, string assetName, Action<T> onLoaded, Action<float> onProgress = null) where T : UnityEngine.Object
        {
            _mgr.LoadAsync<T>(abName, assetName, onLoaded, onProgress);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源路径</param>        
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync(string assetPath, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            string abName;
            string assetName;
            SeparateAssetPath(assetPath, out abName, out assetName);
            _mgr.LoadAsync(abName, assetName, onLoaded, onProgress);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源路径</param>        
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAsync<T>(string assetPath, Action<T> onLoaded, Action<float> onProgress = null) where T : UnityEngine.Object
        {
            string abName;
            string assetName;
            SeparateAssetPath(assetPath, out abName, out assetName);
            _mgr.LoadAsync<T>(abName, assetName, onLoaded, onProgress);
        }

        /// <summary>
        /// 异步获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public void LoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress = null)
        {
            _mgr.LoadAllAsync(abName, onLoaded, onProgress);
        }

        /// <summary>
        /// 将资源所在路径以及资源名合并成一个完整的资源路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string LinkAssetPath(string abName, string assetName)
        {
            if (abName == null)
            {
                abName = "";
            }

            if (assetName == null)
            {
                assetName = "";
            }

            return FileUtility.CombinePaths(abName, assetName);
        }

        /// <summary>
        /// 将一个资源路径拆分为资源父路径以及资源名
        /// </summary>
        /// <param name="assetPath"></param>
        public void SeparateAssetPath(string assetPath, out string abName, out string assetName)
        {
            if (assetPath == null)
            {
                assetPath = "";
            }

            abName = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(assetPath));
            assetName = Path.GetFileName(assetPath);
        }

        /// <summary>
        /// 将AssetBundle和资源名称合并，返回资源的路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string JointAssetPath(string abName, string assetName)
        {
            var assetFolder = FileUtility.CombinePaths(Path.GetDirectoryName(abName) ?? string.Empty, Path.GetFileNameWithoutExtension(abName));
            if (assetFolder == ZeroConst.ROOT_AB_FILE_NAME)
            {
                assetFolder = string.Empty;
            }
            else
            {
                assetFolder += "/";
            }
            
            var assetPath = FileUtility.CombinePaths(assetFolder, assetName);
            return assetPath;
        }

        /// <summary>
        /// 通过AB名称和资源名，获取资源原始路径（在工程Assets中的路径）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string GetOriginalAssetPath(string abName, string assetName)
        {
            var assetPath = FileUtility.CombinePaths(ZeroConst.HOT_RESOURCES_ROOT_DIR, JointAssetPath(abName, assetName));
            return assetPath;
        }

        /// <summary>
        /// 通过资源路径获取资源原始路径（在工程Assets中的路径）
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public string GetOriginalAssetPath(string assetPath)
        {
            //已经是原始地址其实路径，不需要再获取
            if (assetPath.StartsWith(ZeroConst.HOT_RESOURCES_ROOT_DIR))
            {
                return assetPath;
            }
            
            //如果是根AB，则需要忽略根AB的路径
            if (assetPath.StartsWith(ZeroConst.ROOT_AB_FILE_NAME))
            {
                assetPath = Path.GetFileName(assetPath);
            }
            
            var originalAssetPath = FileUtility.CombinePaths(ZeroConst.HOT_RESOURCES_ROOT_DIR, assetPath);
            return originalAssetPath;
        }
    }
}