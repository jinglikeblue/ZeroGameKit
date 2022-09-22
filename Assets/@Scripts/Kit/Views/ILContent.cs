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

            var stage = CreateChildView<StageMgr>("Stage");
            var uiPanel = CreateChildView<UIPanelMgr>("UICanvas/UIPanel");
            var uiWin = CreateChildView<UIWinMgr>("UICanvas/UIWin");

            UIPanelMgr.Ins.Switch<MainStartupPanel>(null);

            Zero.RectTransformUtility.FitRenderSafeArea(uiPanel.GetComponent<RectTransform>(), Screen.safeArea);
            Zero.RectTransformUtility.FitRenderSafeArea(uiWin.GetComponent<RectTransform>(), Screen.safeArea);
        }
    }
}
