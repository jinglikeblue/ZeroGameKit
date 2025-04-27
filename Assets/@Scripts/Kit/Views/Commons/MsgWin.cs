using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;

namespace ZeroGameKit
{
    public class MsgWin : WithCloseButtonWin
    {
        /// <summary>
        /// 显示一个消息窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MsgWin Show(string title, string content, object data = null)
        {
            var win = UIWinMgr.Ins.Open<MsgWin>(data, true, false);
            win.SetTitle(title);
            win.SetContent(content);            
            return win;
        }

        public Button btnSubmit;
       
        public Text textContent;
        public Text textTitle;

        /// <summary>
        /// 确定按钮的委托
        /// </summary>
        public Action onSubmit;

        /// <summary>
        /// 取消按钮的委托
        /// </summary>
        public Action onCancel;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            textContent.text = "";
            textTitle.text = "";
        }

        /// <summary>
        /// 重新设置窗口大小
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void Resize(int w, int h)
        {
            var rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);
        }

        /// <summary>
        /// 设置字体大小
        /// </summary>
        /// <param name="size"></param>
        public void SetFontSize(int size)
        {
            textContent.fontSize = size;
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            textTitle.text = title;
        }

        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="content"></param>
        public void SetContent(string content)
        {
            textContent.text = content;
        }

        /// <summary>
        /// 文本布局形式
        /// </summary>
        /// <param name="type"></param>
        public void SetContentAlignment(TextAnchor value)
        {
            textContent.alignment = value;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            btnSubmit.onClick.RemoveListener(OnBtnSubmitClick);
            btnClose.onClick.RemoveListener(OnBtnCloseClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            btnSubmit.onClick.AddListener(OnBtnSubmitClick);
            btnClose.onClick.AddListener(OnBtnCloseClick);
        }

        protected override void OnBtnCloseClick()
        {
            base.OnBtnCloseClick();
            onCancel?.Invoke();
        }

        private void OnBtnSubmitClick()
        {
            Destroy();
            this.onSubmit?.Invoke();
        }
    }
}