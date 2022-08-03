using Roushan;
using ZeroGameKit;

namespace Example
{
    public class RoushanExample
    {
        public static void Start()
        {
            var msg = MsgWin.Show("肉山大魔王", "一个基于Sprite制作的2D物理游戏");
            msg.onSubmit += () =>
            {
                UIPanelMgr.Ins.Switch<StartupPanel>();
            };
        }
    }
}