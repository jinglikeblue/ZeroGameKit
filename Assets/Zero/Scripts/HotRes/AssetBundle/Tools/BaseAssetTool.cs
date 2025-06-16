using Jing;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源加载抽象基类
    /// </summary>
    public abstract class BaseAssetTool
    {
        /// <summary>
        /// 如果AB名称没有后缀，则加上后缀名
        /// </summary>
        /// <param name="abName"></param>
        protected string ABNameWithExtension(string abName)
        {
            if (abName.StartsWith(ZeroConst.AB_DIR_NAME))
            {
                abName = abName.Substring(ZeroConst.AB_DIR_NAME.Length + 1);
            }
            
            if (false == abName.EndsWith(ZeroConst.AB_EXTENSION))
            {
                abName += ZeroConst.AB_EXTENSION;
            }
            abName = FileUtility.StandardizeBackslashSeparator(abName);
            return abName;
        }

        /// <summary>
        /// 如果AB名称有后缀，则去掉
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        protected string ABNameWithoutExtension(string abName)
        {
            if (abName.StartsWith(ZeroConst.AB_DIR_NAME))
            {
                abName = abName.Substring(ZeroConst.AB_DIR_NAME.Length + 1);
            }
            
            if (abName.EndsWith(ZeroConst.AB_EXTENSION))
            {
                abName = abName.Replace(ZeroConst.AB_EXTENSION, "");
            }
            abName = FileUtility.StandardizeBackslashSeparator(abName);
            return abName;
        }

        /// <summary>
        /// 得到资源的依赖
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract string[] GetDepends(string abName);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isUnloadAllLoaded"></param>
        /// <param name="isUnloadDepends"></param>
        public abstract void Unload(string abName, bool isUnloadAllLoaded = false, bool isUnloadDepends = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isUnloadAllLoaded"></param>
        public abstract void UnloadAll(bool isUnloadAllLoaded = false);

        /// <summary>
        /// 获取AB包下面所有资源的名称
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract string[] GetAllAsssetsNames(string abName);        

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public abstract UnityEngine.Object Load(string abName, string assetName);

        /// <summary>
        /// 获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract UnityEngine.Object[] LoadAll(string abName);

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public abstract T Load<T>(string abName, string assetName) where T : UnityEngine.Object;

        /// <summary>
        /// 异步获取一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public abstract void LoadAsync(string abName, string assetName, Action<UnityEngine.Object> onLoaded, Action<float> onProgress = null);

        /// <summary>
        /// 异步获取一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public abstract void LoadAsync<T>(string abName, string assetName, Action<T> onLoaded, Action<float> onProgress = null) where T:UnityEngine.Object;

        /// <summary>
        /// 异步获取AB下的所有资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="onLoaded"></param>
        /// <param name="onProgress"></param>
        public abstract void LoadAllAsync(string abName, Action<UnityEngine.Object[]> onLoaded, Action<float> onProgress = null);

        /// <summary>
        /// 在运行环境支持的情况下。尝试加载AssetBundle文件。
        /// </summary>
        /// <param name="abName"></param>
        public abstract AssetBundle TryLoadAssetBundle(string abName);
        
        /// <summary>
        /// 在运行环境支持的情况下。尝试异步加载AssetBundle文件。
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract UniTask<AssetBundle> TryLoadAssetBundleAsync(string abName, Action<float> onProgress = null);
    }
}