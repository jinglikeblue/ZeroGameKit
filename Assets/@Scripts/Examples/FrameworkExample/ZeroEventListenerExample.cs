using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class ZeroEventListenerExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<ZeroEventListenerExampleWin>();
        }
    }

    class ZeroEventListenerExampleWin : WithCloseButtonWin
    {
        public Text textLog;

        public GameObject uiEventListenerDemo;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            CreateChildView<DragDemo>(GetChildGameObject("Content/DragDemo"), this);

            L(@"UIEventListener组件实现了UI常用的事件接口，如果要使用更高效的UI事件组件，请使用针对于具体事件的PointerClickEvent等组件。");
            L(@"onDrag事件会与ScrollView组件冲突，请使用onMove替代");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UIEventListener.Get(uiEventListenerDemo).onClick += ClearLog;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            UIEventListener.Get(uiEventListenerDemo).onClick -= ClearLog;
        }

        private void ClearLog(PointerEventData obj)
        {
            textLog.text = "";
        }

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }

        class DragDemo : AView
        {
            bool _isDragging = false;

            ZeroEventListenerExampleWin _win;

            public Text textLocalPos;
            public Text textWorldPos;

            protected override void OnInit(object data)
            {
                base.OnInit(data);

                _win = (ZeroEventListenerExampleWin)data;
            }

            protected override void OnEnable()
            {
                base.OnEnable();

                PointerEnterEventListener.Get(gameObject).onEvent += OnPointerEnter;
                PointerExitEventListener.Get(gameObject).onEvent += OnPointerExit;
                PointerDragEventListener.Get(gameObject).onEvent += OnPointerDrag;
                PointerDownEventListener.Get(gameObject).onEvent += OnPointerDown;
                PointerUpEventListener.Get(gameObject).onEvent += OnPointerUp;

                UpdateEventListener.Get(gameObject).onUpdate += OnUpdate;

            }

            protected override void OnDisable()
            {
                base.OnDisable();

                PointerEnterEventListener.Get(gameObject).onEvent -= OnPointerEnter;
                PointerExitEventListener.Get(gameObject).onEvent -= OnPointerExit;
                PointerDragEventListener.Get(gameObject).onEvent -= OnPointerDrag;
                PointerDownEventListener.Get(gameObject).onEvent -= OnPointerDown;
                PointerUpEventListener.Get(gameObject).onEvent -= OnPointerUp;

                UpdateEventListener.Get(gameObject).onUpdate -= OnUpdate;
            }

            private void OnUpdate()
            {
                textLocalPos.text = $"Local: {transform.localPosition}";
                textWorldPos.text = $"Local: {transform.position}";
            }

            private void OnPointerDrag(PointerEventData obj)
            {
                if (!_isDragging)
                {
                    return;
                }

                transform.position = obj.pointerCurrentRaycast.worldPosition;
                //_win.L(transform.localPosition.ToString());
            }

            private void OnPointerExit(PointerEventData obj)
            {
                _win.L("DragDemo:OnPointerExit");

                _isDragging = false;
            }

            private void OnPointerDown(PointerEventData obj)
            {
                _win.L("DragDemo:OnPointerDown");

                _isDragging = true;
            }

            private void OnPointerUp(PointerEventData obj)
            {
                _win.L("DragDemo:OnPointerUp");

                _isDragging = false;
            }



            private void OnPointerEnter(PointerEventData obj)
            {
                _win.L("DragDemo:OnPointerEnter");
            }
        }
    }
}
