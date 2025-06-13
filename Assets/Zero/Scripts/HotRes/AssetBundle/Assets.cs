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
    public static class Assets
    {
        /// <summary>
        /// 资源加载模式
        /// </summary>
        public enum ELoadMode
        {
            AssetBundle,

            [Obsolete("弃用了，可以使用AssetBundle下的内嵌资源模式替代")]
            Resources,
            AssetDataBase,
            None,
        }

        private static BaseAssetTool _tool;

        private static ELoadMode _mode = ELoadMode.None;

        /// <summary>
        /// 资源管理是否是AssetBundle资源模式
        /// </summary>
        public static bool IsAssetBundle { get; private set; }

        /// <summary>
        /// 资源管理是否是Editor下AssetDataBase接口模式
        /// </summary>
        public static bool IsAssetDatabase { get; private set; }

        static Assets()
        {
            //因为更新了manifest.ab文件，所以要重新初始化ResMgr的Init
            if (Runtime.IsUseAssetDataBase)
            {
                Assets.Init(Assets.ELoadMode.AssetDataBase, ZeroConst.PROJECT_AB_DIR);
            }
            else
            {
                var manifestFileName = ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION;
                Assets.Init(Assets.ELoadMode.AssetBundle, manifestFileName);
            }
        }

        private static void Init(ELoadMode mode, string assetsInfo = null)
        {
            _mode = mode;

            IsAssetBundle = mode == ELoadMode.AssetBundle;
            IsAssetDatabase = mode == ELoadMode.AssetDataBase;

            switch (mode)
            {
                case ELoadMode.AssetBundle:
                    Debug.Log(LogColor.Zero1($"[Zero][ResMgr] 初始化资源管理器... 资源来源：[AssetBundle]  Manifest名称：{assetsInfo}"));
                    AssetBundleTool tool = null;
                    if (WebGL.IsEnvironmentWebGL)
                    {
                        //WebGL需要用提前准备好的文件
                        tool = new AssetBundleTool(WebGL.ManifestAssetBundle);
                    }
                    else
                    {
                        tool = new AssetBundleTool(assetsInfo);
                    }

                    if (_tool != null && _tool is AssetBundleTool)
                    {
                        //替换旧的需要继承一下已加载字典
                        tool.Inherit(_tool as AssetBundleTool);
                    }

                    _tool = tool;
                    break;
                case ELoadMode.Resources:
                    Debug.Log(LogColor.Zero1("[Zero][ResMgr]初始化资源管理器... 资源来源：[Resources]"));
                    _tool = new ResourcesTool();
                    break;
                case ELoadMode.AssetDataBase:
                    Debug.Log(LogColor.Zero1($"[Zero][ResMgr]初始化资源管理器... 资源来源：[AssetDataBase] 资源根目录：{assetsInfo}"));
                    _tool = new AssetDataBaseTool(assetsInfo);
                    break;
                default:
                    throw new Exception("错误的资源模式!");
            }
        }

        /// <summary>
        /// 得到AB资源的依赖。
        /// 返回的依赖清单不需要再递归获取依赖，已为完整依赖列表。
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string[] GetDepends(string abName)
        {
            return _tool.GetDepends(abName);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="abName">资源包名称</param>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        /// <param name="isUnloadDepends">是否卸载关联的资源</param>
        public static void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true)
        {
            _tool.Unload(abName, isUnloadAllLoaded, isUnloadDepends);
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="isUnloadAllLoaded">是否卸载Hierarchy中的资源</param>
        public static void UnloadAll(bool isUnloadAllLoaded = false)
        {
            _tool.UnloadAll(isUnloadAllLoaded);
        }

        /// <summary>
        /// 获取AB中所有资源的名称
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string[] GetAllAsssetsNames(string abName)
        {
            return _tool.GetAllAsssetsNames(abName);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static UnityEngine.Object Load(string abName, string assetName)
        {
            return _tool.Load(abName, assetName);
        }

        /// <summary>
        /// 获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static UnityEngine.Object[] LoadAll(string abName)
        {
            return _tool.LoadAll(abName);
        }

        /// <summary>
        /// 在运行环境支持的情况下。尝试加载AssetBundle文件。
        /// </summary>
        /// <param name="abName"></param>
        public static AssetBundle TryLoadAssetBundle(string abName)
        {
            return _tool.TryLoadAssetBundle(abName);
        }
        
        /// <summary>
        /// 在运行环境支持的情况下。尝试异步加载AssetBundle文件。
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static async UniTask<AssetBundle> TryLoadAssetBundleAsync(string abName)
        {
            return await _tool.TryLoadAssetBundleAsync(abName);
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
            T obj = _tool.Load<T>(abName, assetName);
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

            _tool.LoadAsync(abName, assetName, OnLoaded, onProgress);

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

            _tool.LoadAsync<T>(abName, assetName, Onloaded, onProgress);

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

            _tool.LoadAllAsync(abName, Onloaded, onProgress);

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

            assetPath = Res.TransformToRelativePath(assetPath);

            abName = FileUtility.StandardizeBackslashSeparator(Path.GetDirectoryName(assetPath));
            assetName = Path.GetFileName(assetPath);
        }

        /// <summary>
        /// 通过AB名称和资源名，获取资源原始路径（在工程Assets中的路径）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string GetOriginalAssetPath(string abName, string assetName)
        {
            var assetPath = FileUtility.CombinePaths(ZeroConst.PROJECT_AB_DIR, JointAssetPath(abName, assetName));
            return assetPath;
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
    }
}