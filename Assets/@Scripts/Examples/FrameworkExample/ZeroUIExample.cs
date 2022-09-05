using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;


namespace Example
{
    class ZeroUIExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<ZeroUIExampleWin>();
        }
    }

    class ZeroUIExampleWin : WithCloseButtonWin
    {
        public Button btnMovieClip;
        public Button btnStateImage;
        public Button btnTintTransparentRaycast;
        public Button btnTransparentRaycast;

        public StateUI contents;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            CreateChildView<StateImageDemo>(GetChildGameObject("Contents/StateImageDemo"));            
            CreateChildView<TransparentRaycastDemo>(GetChildGameObject("Contents/TransparentRaycastDemo"));

            contents.SetState(-1);            
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnMovieClip.onClick.AddListener(() =>
            {
                contents.SetState(0);
            });

            btnStateImage.onClick.AddListener(() =>
            {
                contents.SetState(1);
            });

            btnTintTransparentRaycast.onClick.AddListener(() =>
            {
                contents.SetState(2);
            });

            btnTransparentRaycast.onClick.AddListener(() =>
            {
                contents.SetState(3);
            });
        }


        class StateImageDemo : AView
        {
            public StateImage image;

            protected override void OnEnable()
            {
                base.OnEnable();

                UIEventListener.Get(image.gameObject).onPointerDown += (e) =>
                {
                    if (image.State >= image.stateSpriteList.Length)
                    {
                        image.SetState(0);
                    }
                    else
                    {
                        image.SetState(image.State + 1);
                    }
                };
            }
        }

        class TransparentRaycastDemo : AView 
        {
            public TransparentRaycast transparentRaycast;
            public StateImage image;


            protected override void OnEnable()
            {
                base.OnEnable();

                PointerClickEventListener.Get(transparentRaycast.gameObject).onEvent += (e) =>
                {
                    if (image.State >= image.stateSpriteList.Length)
                    {
                        image.SetState(0);
                    }
                    else
                    {
                        image.SetState(image.State + 1);
                    }
                };
            }
        }

    }
}
