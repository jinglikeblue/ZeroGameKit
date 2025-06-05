using UnityEngine;
using ZeroGameKit;

namespace Zero
{
    /// <summary>
    /// 热更代码主类
    /// </summary>
    public class Main
    {
        public Main()
        {
            Signaler.Register(typeof(Main).Assembly);
            
            Signaler.Send(SignalNameDefine.ZeroMainInitBegion);
            Init();
            Signaler.Send(SignalNameDefine.ZeroMainInitEnd);
        }

        void Init()
        {
            Application.targetFrameRate = 240;

            //在左上角显示FPS
            GUIDebugInfo.Show(Color.green, 30);

            if (Application.isEditor)
            {
                PerformanceAnalysis.SetEnable(true);
            }

            //加载ILContent所在的Prefab;
            GameObject mainPrefab = Assets.Load<GameObject>(R.ILContent_prefab);
            //实例化ILContent界面
            var ilContent = ViewFactory.Create<ILContent>(mainPrefab, null);
            ilContent.gameObject.name = mainPrefab.name;
            GameObject.DontDestroyOnLoad(ilContent.gameObject);
        }
    }
}