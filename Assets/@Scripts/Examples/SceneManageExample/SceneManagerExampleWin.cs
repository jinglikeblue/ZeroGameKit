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
        private void OnBtnLoadScene0Click()
        {
            Debug.Log($"加载场景0");
            var temp = ResMgr.Ins.GetOriginalAssetPath(AB.ROOT_ASSETS.ILContent_assetPath);
            var scenePath = ResMgr.Ins.GetOriginalAssetPath(AB.SCENES.Scene0_unity_assetPath);
            SceneManagerUtility.LoadScene(scenePath, LoadSceneMode.Additive);
            var scene = SceneManager.GetSceneByPath(scenePath);
            scene.GetRootGameObjects();
            UIPanelMgr.Ins.Clear();
        }
        
        [BindingButtonClick("BtnUnloadScene0")]
        private void UnloadScene0()
        {
            Debug.Log($"卸载场景0");
            SceneManagerUtility.UnloadSceneAsync(AB.SCENES.Scene0_unity_assetPath);
        }
        
        [BindingButtonClick("BtnLoadScene1")]
        private void OnBtnLoadScene1Click()
        {
            Debug.Log($"加载场景1");
            SceneManagerUtility.LoadScene(AB.SCENES.Scene1_unity_assetPath, LoadSceneMode.Additive);
        }
        
        [BindingButtonClick("BtnUnloadScene1")]
        private void UnloadScene1()
        {
            Debug.Log($"卸载场景1");
            SceneManagerUtility.UnloadSceneAsync(AB.SCENES.Scene1_unity_assetPath);
        }


    }
}