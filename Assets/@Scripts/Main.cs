using UnityEngine;
using Zero;
using ZeroGameKit;

namespace Zero
{
    public class Main
    {
        /// <summary>
        /// 热更代码入口
        /// </summary>
        public static void Startup()
        {
            new Main();
        }

        private Main()
        {
            Init();            
        }

        void Init()
        {
            Application.targetFrameRate = 60;

            //在左上角显示FPS
            GUIDebugInfo.Show();

            if (Application.isEditor)
            {
                PerformanceAnalysis.SetEnable(true);
            }

            //加载ILContent所在的Prefab;
            GameObject mainPrefab = ResMgr.Load<GameObject>(AB.ROOT_ASSETS.NAME, AB.ROOT_ASSETS.ILContent);
            //实例化ILContent界面
            var ilContent = ViewFactory.Create<ILContent>(mainPrefab, null);
            ilContent.gameObject.name = mainPrefab.name;     
            GameObject.DontDestroyOnLoad(ilContent.gameObject);
        }
    }
}
