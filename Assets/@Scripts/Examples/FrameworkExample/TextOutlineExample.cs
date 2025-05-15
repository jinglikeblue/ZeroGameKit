using Zero;
using ZeroGameKit;

namespace Example
{
    class TextOutlineExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<TextOutlineExampleWin>();
        }
    }

    [ViewRegister(R.TextOutlineExampleWin_prefab)]
    class TextOutlineExampleWin : WithCloseButtonWin
    {

    }
}
