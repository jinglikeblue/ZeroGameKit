using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;

namespace Sokoban
{
    public class LevelModule : BaseModule
    {
        /// <summary>
        /// 当前的关卡模型
        /// </summary>
        public LevelModel lv { get; private set; }

        public LevelModule()
        {
            SokobanGlobal.Ins.Notice.onLevelComplete += OnLevelComplete;
        }

        private void OnLevelComplete()
        {
            if (lv.id == Define.LEVEL_AMOUNT)
            {
                EnterLevel(1);
            }
            else
            {
                EnterLevel(lv.id + 1);
            }
        }

        public void EnterLevel(int level)
        {
            lv = new LevelModel(level);
            var loading = UIWinMgr.Ins.Open<LoadingWin>();
            loading.onSwitch += () =>
            {
                UIPanelMgr.Ins.Switch<SokobanGamePanel>();
            };
        }

        public override void Dispose()
        {
            
        }
    }
}
