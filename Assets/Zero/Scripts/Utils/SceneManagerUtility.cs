using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        static SceneManagerUtility()
        {
            IsEditorAPIEnable = Runtime.IsUseAssetDataBase;
            Debug.Log(LogColor.Zero1($"[SceneManagerUtility] 是否使用Editor下的API加载场景: {IsEditorAPIEnable}"));

            // SceneManager.sceneLoaded += OnSceneLoaded;
            // SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        // private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        // {
        //     Debug.Log($"场景加载完成: {scene.path} 加载模式: {mode}");
        // }
        //
        // private static void OnSceneUnloaded(Scene scene)
        // {
        //     Debug.Log($"场景卸载完成: {scene.path} 是否成功移除: {isRemoveSuccess}");
        // }
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
            return GetLoadedSceneList().ToArray();
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

            scenePath = Res.TransformToProjectPath(scenePath, EResType.Asset);
            
            return scenePath;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [Obsolete("通过该接口加载的Scene，要到下一帧的时候，才能拿出GameObject。当前帧scene.GetRootGameObjects().Length为0。")]
        public static Scene LoadScene(string scenePath, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var originalAssetPath = MakePathSafely(scenePath);

            var sceneCountBeforeLoad = SceneManager.sceneCount;

            if (IsEditorAPIEnable)
            {
#if UNITY_EDITOR
                var parameters = new LoadSceneParameters(mode);
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(originalAssetPath, parameters);
#endif
            }
            else
            {
                Assets.SeparateAssetPath(scenePath, out string abName, out string viewName);
                Assets.TryLoadAssetBundle(abName);
                SceneManager.LoadScene(originalAssetPath, mode);
            }

            var sceneCountAfterLoad = SceneManager.sceneCount;

            Scene scene = default;
            if (mode == LoadSceneMode.Additive && sceneCountBeforeLoad == sceneCountAfterLoad)
            {
                //场景数量没有增加，说明加载失败
            }
            else if (mode == LoadSceneMode.Single && sceneCountBeforeLoad != 1)
            {
                //场景数量不为1，一定是加载失败
            }
            else
            {
                var b = GetLoadedSceneList();
                scene = b.LastOrDefault(x => x.path == originalAssetPath);
            }

            if (scene.IsValid())
            {
                Debug.Log($"[SceneManagerUtility] 加载场景成功: {originalAssetPath} 模式:{mode} 根节点数:{scene.GetRootGameObjects().Length}");
            }
            else
            {
                Debug.Log(LogColor.Red($"[SceneManagerUtility] 加载场景失败: {originalAssetPath} 模式:{mode}"));
            }

            return scene;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        public static async UniTask<Scene> LoadSceneAsync(string scenePath, LoadSceneMode mode = LoadSceneMode.Single, Action<float> onProgress = null)
        {
            var originalAssetPath = MakePathSafely(scenePath);

            AsyncOperation ao = null;
            if (IsEditorAPIEnable)
            {
#if UNITY_EDITOR
                var parameters = new LoadSceneParameters(mode);
                ao = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(originalAssetPath, parameters);
#endif
            }
            else
            {
                Assets.SeparateAssetPath(scenePath, out string abName, out string viewName);
                Assets.TryLoadAssetBundle(abName);
                ao = SceneManager.LoadSceneAsync(originalAssetPath, mode);
            }

            Scene scene = default;
            if (null != ao)
            {
                while (false == ao.isDone)
                {
                    Debug.Log($"[SceneManagerUtility] 异步加载场景: {originalAssetPath} 模式:{mode} 进度: {ao.progress}");
                    onProgress?.Invoke(ao.progress);
                    await UniTask.NextFrame();
                }
                
                Debug.Log($"[SceneManagerUtility] 异步加载场景: {originalAssetPath} 模式:{mode} 进度: {ao.progress}");
                onProgress?.Invoke(ao.progress);
                await UniTask.NextFrame();

                var b = GetLoadedSceneList();
                scene = b.LastOrDefault(x => x.path == originalAssetPath);
            }

            if (scene.IsValid())
            {
                Debug.Log($"[SceneManagerUtility] 异步加载场景成功: {originalAssetPath} 模式:{mode} 根节点数:{scene.GetRootGameObjects().Length}");
            }
            else
            {
                Debug.Log(LogColor.Red($"[SceneManagerUtility] 异步加载场景失败: {originalAssetPath} 模式:{mode}"));
            }

            return scene;
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public static async UniTask UnloadSceneAsync(string scenePath)
        {
            scenePath = MakePathSafely(scenePath);
            
            AsyncOperation ao = null;
            try
            {
                ao = SceneManager.UnloadSceneAsync(scenePath);
            }
            catch (Exception e)
            {
                Debug.Log(LogColor.Red($"[SceneManagerUtility] 卸载场景失败: {scenePath}"));
            }
            
            if (null != ao)
            {
                await ao.ToUniTask();
                Debug.Log($"[SceneManagerUtility] 卸载场景: {scenePath}");
            }
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
                if (gameObjectName.Equals(rootGameObject.name))
                {
                    return rootGameObject;
                }

                if (gameObjectName.StartsWith(rootGameObject.name))
                {
                    var childName = gameObjectName.Substring(rootGameObject.name.Length + 1);
                    return rootGameObject.transform.Find(childName).gameObject;
                }
            }

            return null;
        }
    }
}