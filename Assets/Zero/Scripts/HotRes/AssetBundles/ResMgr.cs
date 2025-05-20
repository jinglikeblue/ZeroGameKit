using Jing;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public static class ResMgr
    {
        public enum EResMgrType
        {
            AssetBundle,

            [Obsolete("弃用了，可以使用AssetBundle下的内嵌资源模式替代")]
            Resources,
            AssetDataBase,
            None,
        }

        private static AResMgr _mgr;

        private static EResMgrType _type = EResMgrType.None;

        /// <summary>
        /// 资源管理是否是AssetBundle资源模式
        /// </summary>
        public static bool IsAssetBundle { get; private set; }

        /// <summary>
        /// 资源管理是否是Editor下AssetDataBase接口模式
        /// </summary>
        public static bool IsAssetDatabase { get; private set; }

        public static void Init(EResMgrType type, string assetsInfo = null)
        {
            _type = type;

            IsAssetBundle = type == EResMgrType.AssetBundle;
            IsAssetDatabase = type == EResMgrType.AssetDataBase;

            switch (type)
            {
                case EResMgrType.AssetBundle:
                    Debug.Log(LogColor.Zero1($"[Zero][ResMgr] 初始化资源管理器... 资源来源：[AssetBundle]  Manifest名称：{assetsInfo}"));
                    var newMgr = new AssetBundleResMgr(assetsInfo);
                    if (_mgr != null && _mgr is AssetBundleResMgr)
                    {
                        //替换旧的需要继承一下已加载字典
                        newMgr.Inherit(_mgr as AssetBundleResMgr);
                    }

                    _mgr = newMgr;
                    break;
                case EResMgrType.Resources:
                    Debug.Log(LogColor.Zero1("[Zero][ResMgr]初始化资源管理器... 资源来源：[Resources]"));
                    _mgr = new ResourcesResMgr();
                    break;
                case EResMgrType.AssetDataBase:
                    Debug.Log(LogColor.Zero1($"[Zero][ResMgr]初始化资源管理器... 资源来源：[AssetDataBase] 资源根目录：{assetsInfo}"));
                    _mgr = new AssetDataBaseResMgr(assetsInfo);
                    break;
                default:
                    throw new Exception("错误的资源模式!");
            }
        }

        /// <summary>
        /// 执行一次内存回收(该接口开销大，可能引起卡顿)
        /// </summary>
        public static void DoGC()
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
        public static string[] GetDepends(string abName)
        {
            return _mgr.GetDepends(abName);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abName">资源包名称</param>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        /// <param name="isUnloadDepends">是否卸载关联的资源</param>
        public static void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            _mgr.Unload(abName, isUnloadAllLoaded, isUnloadDepends);
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        public static void UnloadAll(bool isUnloadAllLoaded = false)
        {
            _mgr.UnloadAll(isUnloadAllLoaded);
        }

        /// <summary>
        /// 获取AB中所有资源的名称
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string[] GetAllAsssetsNames(string abName)
        {
            return _mgr.GetAllAsssetsNames(abName);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static UnityEngine.Object Load(string abName, string assetName)
        {
            return _mgr.Load(abName, assetName);
        }

        /// <summary>
        /// 获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static UnityEngine.Object[] LoadAll(string abName)
        {
            return _mgr.LoadAll(abName);
        }

        /// <summary>
        /// 在运行环境支持的情况下。尝试加载AssetBundle文件。
        /// </summary>
        /// <param name="abName"></param>
        public static AssetBundle TryLoadAssetBundle(string abName)
        {
            return _mgr.TryLoadAssetBundle(abName);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static UnityEngine.Object Load(string assetPath)
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
        public static T Load<T>(string abName, string assetName) where T : UnityEngine.Object
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
        public static T Load<T>(string assetPath) where T : UnityEngine.Object
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
        public static UniTask<UnityEngine.Object> LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null)
        {
            var completionSource = new UniTaskCompletionSource<UnityEngine.Object>();

            _mgr.LoadAsync(abName, assetName, OnLoaded, onProgress);

            void OnLoaded(UnityEngine.Object obj)
            {
                onLoaded?.Invoke(obj);
                completionSource.TrySetResult(obj);
            }

            return completionSource.Task;
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">资源包名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public static UniTask<T> LoadAsync<T>(string abName, string assetName, Action<T> onLoaded = null, Action<float> onProgress = null) where T : UnityEngine.Object
        {
            var completionSource = new UniTaskCompletionSource<T>();

            _mgr.LoadAsync<T>(abName, assetName, Onloaded, onProgress);

            void Onloaded(T obj)
            {
                onLoaded?.Invoke(obj);
                completionSource.TrySetResult(obj);
            }

            return completionSource.Task;
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源路径</param>        
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public static UniTask<UnityEngine.Object> LoadAsync(string assetPath, Action<UnityEngine.Object> onLoaded = null, Action<float> onProgress = null)
        {
            SeparateAssetPath(assetPath, out var abName, out var assetName);
            return LoadAsync(abName, assetName, onLoaded, onProgress);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源路径</param>        
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public static UniTask<T> LoadAsync<T>(string assetPath, Action<T> onLoaded = null, Action<float> onProgress = null) where T : UnityEngine.Object
        {
            SeparateAssetPath(assetPath, out var abName, out var assetName);
            return LoadAsync<T>(abName, assetName, onLoaded, onProgress);
        }

        /// <summary>
        /// 异步获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public static UniTask<UnityEngine.Object[]> LoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded = null, Action<float> onProgress = null)
        {
            var completionSource = new UniTaskCompletionSource<UnityEngine.Object[]>();

            _mgr.LoadAllAsync(abName, Onloaded, onProgress);

            void Onloaded(UnityEngine.Object[] objs)
            {
                onLoaded?.Invoke(objs);
                completionSource.TrySetResult(objs);
            }

            return completionSource.Task;
        }

        /// <summary>
        /// 将资源所在路径以及资源名合并成一个完整的资源路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string LinkAssetPath(string abName, string assetName)
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
        public static void SeparateAssetPath(string assetPath, out string abName, out string assetName)
        {
            if (assetPath == null)
            {
                assetPath = "";
            }

            assetPath = RemoveRootFolder(assetPath);

            abName = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(assetPath));
            assetName = Path.GetFileName(assetPath);
        }

        /// <summary>
        /// 将AssetBundle和资源名称合并，返回资源的路径
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string JointAssetPath(string abName, string assetName)
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
        public static string GetOriginalAssetPath(string abName, string assetName)
        {
            var assetPath = FileUtility.CombinePaths(ZeroConst.HOT_AB_ROOT_DIR, JointAssetPath(abName, assetName));
            return assetPath;
        }

        /// <summary>
        /// 通过资源路径获取资源原始路径（在工程Assets中的路径）
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetOriginalAssetPath(string assetPath)
        {
            //已经是原始地址其实路径，不需要再获取
            if (assetPath.StartsWith(ZeroConst.HOT_AB_ROOT_DIR))
            {
                return assetPath;
            }

            //如果是根AB，则需要忽略根AB的路径
            if (assetPath.StartsWith(ZeroConst.ROOT_AB_FILE_NAME))
            {
                assetPath = Path.GetFileName(assetPath);
            }

            var originalAssetPath = FileUtility.CombinePaths(ZeroConst.HOT_AB_ROOT_DIR, assetPath);
            return originalAssetPath;
        }

        /// <summary>
        /// 通过资源原始路径获取
        /// </summary>
        /// <param name="originalAssetPath"></param>
        /// <returns></returns>
        public static string GetAssetPathFromOriginal(string originalAssetPath)
        {
            if (!originalAssetPath.StartsWith(ZeroConst.HOT_AB_ROOT_DIR))
            {
                return originalAssetPath;
            }

            var assetPath = originalAssetPath.Substring(ZeroConst.HOT_AB_ROOT_DIR.Length + 1);
            return assetPath;
        }

        /// <summary>
        /// 添加AB资源根目录
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AddRootFolder(string resPath)
        {
            if (!resPath.StartsWith(ZeroConst.AB_DIR_NAME))
            {
                resPath = FileUtility.CombinePaths(ZeroConst.AB_DIR_NAME, resPath);
            }

            return resPath;
        }

        /// <summary>
        /// 移除资源根目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveRootFolder(string path)
        {
            if (path.StartsWith(ZeroConst.HOT_AB_ROOT_DIR))
            {
                path = path.Remove(0, ZeroConst.HOT_AB_ROOT_DIR.Length + 1);
            }
            else if (path.StartsWith(ZeroConst.AB_DIR_NAME))
            {
                path = path.Remove(0, ZeroConst.HOT_AB_ROOT_DIR.Length + 1);
            }

            return path;
        }
    }
}