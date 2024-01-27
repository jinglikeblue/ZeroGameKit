using System.Text;
using UnityEngine;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class UnityConstsExample
    {
        public static void Start()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(LogColor.Zero1("------------------Screen------------------"));
            sb.AppendLine($"Screen.width = {Screen.width} , Screen.height = {Screen.height}");
            sb.AppendLine($"Screen.safeArea = {Screen.safeArea}");
            sb.AppendLine();

            var content = sb.ToString();

            var msg = MsgWin.Show("Unity常用参数", content);
            var canvas = msg.gameObject.GetComponentInParent<ZeroUICanvas>();
            msg.Resize((int)(canvas.RenderWidth * 0.9), (int)(canvas.RenderHeight * 0.9));
        }
    }
}
