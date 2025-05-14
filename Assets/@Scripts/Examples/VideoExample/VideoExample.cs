using RenderHeads.Media.AVProVideo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;

namespace Example
{
    class VideoExample
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<VideoExampleWin>();
        }
    }

    class VideoExampleWin : WithCloseButtonWin
    {
        public MediaPlayer content;
        public Text textPath;

        protected override async void OnInit(object data)
        {
            base.OnInit(data);
            
            var videoPath = Res.AbsolutePath("videos/Sample.mp4");
            textPath.text = videoPath;
            content.OpenMedia(MediaPathType.AbsolutePathOrURL, videoPath, true);

            
        }
    }
}
