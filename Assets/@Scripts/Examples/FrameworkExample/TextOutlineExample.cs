using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;
using ZeroHot;

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
