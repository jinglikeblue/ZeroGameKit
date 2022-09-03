using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    class LoadingWin:AView
    {
        public event Action onSwitch;
        public event Action onOver;

        AnimationCallback _acb;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _acb = GetComponent<AnimationCallback>();            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _acb.onCallback += OnCallback;
        }        

        protected override void OnDisable()
        {
            base.OnDisable();
            _acb.onCallback -= OnCallback;
        }

        private void OnCallback(string tag)
        {
            switch (tag)
            {
                case "switch":
                    onSwitch?.Invoke();
                    break;
                case "over":
                    onOver?.Invoke();
                    Destroy();
                    break;
            }
        }
    }
}
