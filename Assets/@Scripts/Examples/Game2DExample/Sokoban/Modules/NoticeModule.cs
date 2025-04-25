using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;

namespace Sokoban
{
    public class NoticeModule : BaseModule
    {
        public Action onScreenSizeChange;

        /// <summary>
        /// 关卡完成
        /// </summary>
        public Action onLevelComplete;

        public override void Dispose()
        {
            onScreenSizeChange = null;
            onLevelComplete = null;
        }
    }
}
