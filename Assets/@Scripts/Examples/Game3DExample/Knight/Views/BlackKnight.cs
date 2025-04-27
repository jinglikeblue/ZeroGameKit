using UnityEngine;
using Zero;

namespace Knight
{
    class BlackKnight : AView
    {
        public Canvas canvas;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            canvas = GetChildComponent<Canvas>("Canvas");
        }
    }
}
