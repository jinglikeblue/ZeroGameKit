using Jing;
using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace ZeroGameKit
{
    /// <summary>
    /// 启动面板
    /// </summary>
    public class MainStartupPanel : AView
    {
        Text textState;
        Text textProgress;
        Image bar;

        string[] startupRes = new string[] { "/" };

        protected override async void OnInit(object data)
        {
            OnUpdaterProgress(0, 0,1);
            
            var err = await Res.UpdateGroup(startupRes, OnUpdaterProgress);
            if (string.IsNullOrEmpty(err))
            {
                Enter();
            }
            else
            {
                textState.text = err;
            }
        }

        private void OnUpdaterProgress(float progress, long loadedSize, long totalSize)
        {
            //转换为MB
            float totalMB = totalSize / 1024 / 1024f;
            float loadedMB = loadedSize / 1024 / 1024f;
            textProgress.text = string.Format("{0}% [{1}MB/{2}MB]", (int)(progress * 100f), loadedMB.ToString("0.00"), totalMB.ToString("0.00"));
            bar.fillAmount = progress;
        }

        void Enter()
        {
            UIPanelMgr.Ins.SwitchAsync<MenuPanel>(null, OnCreated, OnProgress);
        }

        private void OnProgress(float progress)
        {
            if (float.IsNaN(progress))
            {
                progress = 0;
            } 
            Debug.Log("[MainStartupPanel]加载进度：" + progress);
        }

        private void OnCreated(AView view)
        {
            Debug.Log("[MainStartupPanel]创建完成:" + view.gameObject.name);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}