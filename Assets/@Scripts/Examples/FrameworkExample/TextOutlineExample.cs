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

    [ViewRegister(AB.EXAMPLES_TEXT_OUTLINE.TextOutlineExampleWin_assetPath)]
    class TextOutlineExampleWin : WithCloseButtonWin
    {

    }
}
