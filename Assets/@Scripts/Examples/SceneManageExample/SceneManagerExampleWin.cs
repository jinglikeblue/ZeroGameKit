using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zero;
using ZeroGameKit;
using ZeroHot;
using Debug = behaviac.Debug;

namespace Example
{
    /// <summary>
    /// 场景管理器示例
    /// </summary>
    [ViewRegister(AB.EXAMPLES_SCENE.SceneManagerExampleWin_assetPath)]
    public class SceneManagerExampleWin : WithCloseButtonWin
    {
        public static void Show()
        {
            // foreach (var scene in SceneManagerUtility.GetLoadedScenes())
            // {
            //     Debug.Log($"移除Scene:{scene.path}");
            //     SceneManager.UnloadSceneAsync(scene.path);
            // }
            UIWinMgr.Ins.Open<SceneManagerExampleWin>(null, true, false);
        }

        private CanvasGroup _canvasGroup;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _canvasGroup.alpha = 0.5f;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UIPanelMgr.Ins.Switch<MenuPanel>();
        }

        [BindingButtonClick("BtnLoadScene0")]
        private async void LoadScene0()
        {
            Debug.Log($"加载场景0");
            var scene = SceneManagerUtility.LoadScene(AB.SCENES.Scene0_unity_assetPath, LoadSceneMode.Additive);
            await UniTask.NextFrame();
            var rootObjs = scene.GetRootGameObjects();
            var view1 = ViewFactory.Binding<SceneGameObjectView>(scene.FindGameObject("Main Camera"));
            ViewFactory.Binding<SceneGameObjectView>(scene.FindGameObject("Main Camera/Cube"));
        }

        [BindingButtonClick("BtnAsyncLoadScene0")]
        private void AsyncLoadScene0()
        {
            Debug.Log($"异步加载场景0");
            SceneManagerUtility.LoadSceneAsync(AB.SCENES.Scene0_unity_assetPath, LoadSceneMode.Additive);
        }

        [BindingButtonClick("BtnUnloadScene0")]
        private void UnloadScene0()
        {
            Debug.Log($"卸载场景0");
            SceneManagerUtility.UnloadSceneAsync(AB.SCENES.Scene0_unity_assetPath);
        }

        [BindingButtonClick("BtnSwitchScene0")]
        private void SwitchScene0()
        {
            Debug.Log($"切换场景0");
            SceneManagerUtility.LoadScene(AB.SCENES.Scene0_unity_assetPath);
        }

        [BindingButtonClick("BtnLoadScene1")]
        private async void LoadScene1()
        {
            Debug.Log($"加载场景1");
            SceneManagerUtility.LoadScene(AB.SCENES.Scene1_unity_assetPath, LoadSceneMode.Additive);
            await UniTask.NextFrame();
            var view = ViewFactory.Binding<SceneGameObjectView>(GameObject.Find("Main Camera"));
            if (null == view)
            {
                Debug.LogError($"Biding View失败: Main Camer");
            }
        }

        [BindingButtonClick("BtnAsyncLoadScene1")]
        private void AsyncLoadScene1()
        {
            Debug.Log($"异步加载场景1");
            SceneManagerUtility.LoadSceneAsync(AB.SCENES.Scene1_unity_assetPath, LoadSceneMode.Additive);
        }

        [BindingButtonClick("BtnUnloadScene1")]
        private void UnloadScene1()
        {
            Debug.Log($"卸载场景1");
            SceneManagerUtility.UnloadSceneAsync(AB.SCENES.Scene1_unity_assetPath);
        }

        [BindingButtonClick("BtnSwitchScene1")]
        private void SwitchScene1()
        {
            Debug.Log($"切换场景1");
            SceneManagerUtility.LoadScene(AB.SCENES.Scene1_unity_assetPath);
        }

        /// <summary>
        /// 检查所有场景
        /// </summary>
        [BindingButtonClick("BtnCheckScenes")]
        private void CheckScenes()
        {
            // 遍历所有已加载的场景
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"场景[{i}]: {scene.name}, 已加载: {scene.isLoaded}, 构建索引: {scene.buildIndex}");

                // 获取场景中所有根游戏对象
                GameObject[] rootObjects = scene.GetRootGameObjects();
                Debug.Log($"包含{rootObjects.Length}个根对象");
            }
        }

        /// <summary>
        /// 清理Additive方式加载的场景
        /// </summary>
        [BindingButtonClick("BtnCleanAddScenes")]
        private async void CleanAddScenes()
        {
            try
            {
                var scenes = SceneManagerUtility.GetLoadedScenes();
                for (int i = 1; i < scenes.Length; i++)
                {
                    var ao = SceneManagerUtility.UnloadSceneAsync(scenes[i].path);
                    await ao.ToUniTask();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        [BindingButtonClick("BtnCleanScenes")]
        async void CleanScenes()
        {
            try
            {
                var scenes = SceneManagerUtility.GetLoadedScenes();
                for (int i = 0; i < scenes.Length; i++)
                {
                    var ao = SceneManagerUtility.UnloadSceneAsync(scenes[i].path);
                    while (!ao.isDone)
                    {
                        await UniTask.NextFrame();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        [BindingButtonClick("BtnLoadWrong")]
        void LoadWrongScene()
        {
            SceneManagerUtility.LoadScene("WrongScene.unity", LoadSceneMode.Single);
            // SceneManagerUtility.LoadSceneAsync("WrongSceneAsync.unity", LoadSceneMode.Additive);
        }
    }
}