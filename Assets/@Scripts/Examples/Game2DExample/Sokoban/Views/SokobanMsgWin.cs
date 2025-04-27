using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    class SokobanMsgWin : WithCloseButtonWin
    {
        public static SokobanMsgWin Show(string content, bool isConfirm = false, Action onOK = null, Action onCancel = null)
        {
            var msgWin = UIWinMgr.Ins.Open<SokobanMsgWin>();
            msgWin.Set(content, isConfirm, onOK, onCancel);
            return msgWin;
        }

        Action _onOK;
        Action _onCancel;

        Button _btnOK;
        Text _text;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _btnOK = GetChildComponent<Button>("Panel/Buttons/BtnOK");
            _text = GetChildComponent<Text>("Panel/Text");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _btnOK.onClick.AddListener(OK);            
        }

        public void SetLabel(string labelOK, string labelCancel = "")
        {
            _btnOK.GetComponentInChildren<Text>().text = labelOK;
            btnClose.GetComponentInChildren<Text>().text = labelCancel;
        }

        public void Set(string content, bool isConfirm = false, Action onOK = null, Action onCancel = null)
        {
            btnClose.gameObject.SetActive(isConfirm);
            _text.text = content;
            _onOK = onOK;
            _onCancel = onCancel;
        }

        protected override void OnBtnCloseClick()
        {
            base.OnBtnCloseClick();

            _onCancel?.Invoke();
            Close();
        }

        private void OK()
        {
            _onOK?.Invoke();
            Close();
        }

        void Close()
        {
            _onOK = null;
            _onCancel = null;
        }
    }
}
