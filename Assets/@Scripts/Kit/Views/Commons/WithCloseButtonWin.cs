using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using Zero;
using DG.Tweening;
using System;

namespace ZeroGameKit
{
    /// <summary>
    /// 带关闭按钮的窗口，窗口的关闭按钮必须命名为「BtnClose」
    /// </summary>
    public class WithCloseButtonWin : AView
    {
        public Button btnClose;

        /// <summary>
        /// 界面出厂动画完成后触发
        /// </summary>
        public event Action onShow;

        /// <summary>
        /// 是否允许特效执行
        /// </summary>
        protected bool effectEnable = true;

        const float TWEEN_TIME = 0.3f;

        protected override void OnDisable()
        {
            base.OnDisable();
            // btnClose.onClick.RemoveListener(OnBtnCloseClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // btnClose.onClick.AddListener(OnBtnCloseClick);

            if (effectEnable)
            {
                var gr = ComponentUtil.AutoGet<GraphicRaycaster>(gameObject);
                gr.enabled = false;

                //入场动画
                transform.DOLocalMoveY(0, TWEEN_TIME).From(-100);
                var cg = ComponentUtil.AutoGet<CanvasGroup>(gameObject);
                cg.DOFade(1, TWEEN_TIME).From(0).OnComplete(() =>
                {
                    gr.enabled = true;
                    onShow?.Invoke();
                });

                //背景遮罩淡入
                var blurImg = UIWinMgr.Ins.Blur.GetComponent<Image>();
                blurImg.DOFade(blurImg.color.a, TWEEN_TIME).From(0);
            }
        }

        [BindingButtonClick("BtnClose")]
        protected virtual void OnBtnCloseClick()
        {
            if (effectEnable)
            {
                ComponentUtil.AutoGet<GraphicRaycaster>(gameObject).enabled = false;

                //出场动画
                transform.DOLocalMoveY(+100, TWEEN_TIME);
                var cg = ComponentUtil.AutoGet<CanvasGroup>(gameObject);
                cg.DOFade(0, TWEEN_TIME).OnComplete(() =>
                {
                    Destroy();
                });

                //背景遮罩淡出
                var blurImg = UIWinMgr.Ins.Blur.GetComponent<Image>();
                var oldAlpha = blurImg.color.a;
                blurImg.DOFade(0, TWEEN_TIME).OnComplete(() =>
                {
                    UIWinMgr.Ins.Blur.SetAlpha(oldAlpha);
                });
            }
            else
            {
                Destroy();
            }
        }
    }
}