using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zero
{
    /// <summary>
    /// 场景管理器的工具类
    /// </summary>
    public static class SceneManagerUtility
    {
        /// <summary>
        /// 是否允许使用Editor下的API加载场景
        /// </summary>
        public static readonly bool IsEditorAPIEnable;

        /// <summary>
        /// 已加载的场景列表。索引顺序跟加载顺序关联。默认索引0位的为Single方式加载的。其它都是Additive方式加载的。
        /// </summary>
        private static List<Scene> _loadedSceneList = null;

        static SceneManagerUtility()
        {
            IsEditorAPIEnable = Runtime.Ins.HotResMode == EHotResMode.ASSET_DATA_BASE;
            Debug.Log(LogColor.Zero1($"[SceneManagerUtility] 是否允许使用Editor下的API加载场景: {IsEditorAPIEnable}"));

            //初始化场景列表
            _loadedSceneList = GetLoadedSceneList();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            // SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"场景加载完成: {scene.path} 加载模式: {mode}");

            if (LoadSceneMode.Single == mode)
            {
                _loadedSceneList.Clear();
            }

            _loadedSceneList.Add(scene);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            var isRemoveSuccess = _loadedSceneList.Remove(scene);
            Debug.Log($"场景卸载完成: {scene.path} 是否成功移除: {isRemoveSuccess}");
        }
        //
        // private static void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        // {
        //     Debug.Log($"切换前场景: {previousScene.path} 场景切换完成: {newScene.path} ");
        // }

        /// <summary>
        /// 获取Additive
        /// </summary>
        /// <returns></returns>
        public static Scene[] GetLoadedScenes()
        {
            return _loadedSceneList.ToArray();
        }

        /// <summary>
        /// 确保一个安全的场景路径。路径为资源的原始路径。
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string MakePathSafely(string scenePath)
        {
            if (!scenePath.EndsWith(".unity"))
            {
                throw new Exception($"错误的场景路径: {scenePath}");
            }

            scenePath = ResMgr.Ins.GetOriginalAssetPath(scenePath);

            return scenePath;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Scene LoadScene(string scenePath, LoadSceneMode mode = LoadSceneMode.Single)
        {
            scenePath = MakePathSafely(scenePath);

            Debug.Log($"加载场景: {scenePath} 模式:{mode}");

            if (IsEditorAPIEnable)
            {
#if UNITY_EDITOR
                var parameters = new LoadSceneParameters(mode);
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scenePath, parameters);
#endif
            }
            else
            {
                SceneManager.LoadScene(scenePath, mode);
            }

            var scene = SceneManager.GetSceneByPath(scenePath);
            return scene;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static AsyncOperation LoadSceneAsync(string scenePath, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation ao = null;
            
            scenePath = MakePathSafely(scenePath);

            Debug.Log($"异步加载场景: {scenePath} 模式:{mode}");

            if (IsEditorAPIEnable)
            {
#if UNITY_EDITOR
                var parameters = new LoadSceneParameters(mode);
                ao = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, parameters);
                return ao;
#endif
            }

            ao = SceneManager.LoadSceneAsync(scenePath, mode);
            return ao;
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public static AsyncOperation UnloadSceneAsync(string scenePath)
        {
            scenePath = MakePathSafely(scenePath);

            Debug.Log($"卸载场景: {scenePath}");

            return SceneManager.UnloadSceneAsync(scenePath);
        }

        /// <summary>
        /// 获取已加载的场景数组
        /// </summary>
        /// <returns></returns>
        private static List<Scene> GetLoadedSceneList()
        {
            List<Scene> sceneList = new List<Scene>();
            // 遍历所有已加载的场景
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                sceneList.Add(scene);
            }

            return sceneList;
        }

        /// <summary>
        /// 查找指定场景中的GameObject
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(string scenePath, string gameObjectName)
        {
            scenePath = MakePathSafely(scenePath);
            var scene = SceneManager.GetSceneByPath(scenePath);
            return FindGameObject(scene, gameObjectName);
        }

        /// <summary>
        /// 查找指定场景下的GameObject
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(Scene scene, string gameObjectName)
        {
            if (false == scene.IsValid() || string.IsNullOrEmpty(gameObjectName))
            {
                return null;
            }
            
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                if (rootGameObject.name.Equals(gameObjectName))
                {
                    return rootGameObject;
                }

                if (gameObjectName.StartsWith(rootGameObject.name))
                {
                    return rootGameObject.transform.Find(gameObjectName).gameObject;
                }
            }

            return null;
        }
    }
}