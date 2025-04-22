using System;
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
        public static bool isEditorAPIEnable = true;

        static SceneManagerUtility()
        {
            // SceneManager.sceneLoaded += OnSceneLoaded;
            // SceneManager.sceneUnloaded += OnSceneUnloaded;
            // SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        // private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        // {
        //     Debug.Log($"场景加载完成: {scene.name} 加载模式: {mode}");
        // }
        //
        // private static void OnSceneUnloaded(Scene scene)
        // {
        //     Debug.Log($"场景卸载完成: {scene.name}");
        // }
        //
        // private static void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        // {
        //     Debug.Log($"切换前场景: {previousScene.path} 场景切换完成: {newScene.path} ");
        // }

        /// <summary>
        /// 确保一个安全的场景路径
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

            if (!scenePath.StartsWith("Assets/"))
            {
                scenePath = $"Assets/{scenePath}";
            }

            return scenePath;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static void LoadScene(string scenePath, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Debug.Log($"加载场景: {scenePath} 模式:{mode}");

            scenePath = MakePathSafely(scenePath);

#if UNITY_EDITOR
            if (isEditorAPIEnable)
            {
                var parameters = new LoadSceneParameters(mode);
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scenePath, parameters);
                return;
            }
#endif

            SceneManager.LoadScene(scenePath, mode);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static AsyncOperation LoadSceneAsync(string scenePath, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Debug.Log($"异步加载场景: {scenePath} 模式:{mode}");

            scenePath = MakePathSafely(scenePath);

#if UNITY_EDITOR
            if (isEditorAPIEnable)
            {
                var parameters = new LoadSceneParameters(mode);
                return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, parameters);
            }
#endif

            return SceneManager.LoadSceneAsync(scenePath, mode);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public static AsyncOperation UnloadSceneAsync(string scenePath)
        {
            Debug.Log($"卸载场景: {scenePath}");

            scenePath = MakePathSafely(scenePath);

            return SceneManager.UnloadSceneAsync(scenePath);
        }
    }
}