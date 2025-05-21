using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    class TexturesExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Show<TexturesWin>();
        }
    }

    class TexturesWin : WithCloseButtonWin
    {
        public Image dynamicImage;

        protected override void OnEnable()
        {
            base.OnEnable();
            dynamicImage.sprite = Assets.Load<Sprite>(R.item_01_png);
        }
    }
}
