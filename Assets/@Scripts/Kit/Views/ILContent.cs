using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroHot;

namespace ZeroGameKit
{
    public class ILContent : AView
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);

            CameraMgr.Ins.RegisterUICamera(transform.Find("UICamera").GetComponent<Camera>());

            var stage = CreateChildView<StageMgr>("Stage");
            var uiPanel = CreateChildView<UIPanelMgr>("UICanvas/UIPanel");
            var uiWin = CreateChildView<UIWinMgr>("UICanvas/UIWin");

            UIPanelMgr.Ins.Switch<MainStartupPanel>(null);
        }
    }
}
