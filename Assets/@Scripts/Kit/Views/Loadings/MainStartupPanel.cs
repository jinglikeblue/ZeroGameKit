using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
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

        protected override void OnInit(object data)
        {
            OnUpdaterProgress(0, 1);
            HotResUpdater updater = new HotResUpdater(startupRes);
            updater.onComplete += OnUpdaterComplete;
            updater.onProgress += OnUpdaterProgress;
            updater.Start();            
        }

        private void OnUpdaterComplete(BaseUpdater updater)
        {
            if(updater.error != null)
            {
                textState.text = updater.error;
            }
            else
            {
                Enter();
            }
        }

        private void OnUpdaterProgress(long loadedSize, long totalSize)
        {
            //转换为MB
            float totalMB = totalSize / 1024 / 1024f;
            float loadedMB = loadedSize / 1024 / 1024f;
            float progress = loadedMB / totalMB;
            textProgress.text = string.Format("{0}% [{1}MB/{2}MB]", (int)(progress * 100f), loadedMB.ToString("0.00"), totalMB.ToString("0.00"));
            bar.fillAmount = progress;
        }

        void Enter()
        {
            UIPanelMgr.Ins.SwitchAsync<MenuPanel>(null, OnCreated, OnProgress);
        }

        private void OnProgress(float progress)
        {
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