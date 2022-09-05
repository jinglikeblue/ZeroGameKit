using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    class BangEffect:AView
    {
        AnimationCallback _acb;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _acb = GetComponent<AnimationCallback>();            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _acb.onCallback += OnCallBack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _acb.onCallback -= OnCallBack;
        }

        private void OnCallBack(string obj)
        {
            Destroy();
        }
    }
}
