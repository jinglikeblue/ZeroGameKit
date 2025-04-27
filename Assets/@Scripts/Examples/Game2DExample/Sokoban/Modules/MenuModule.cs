using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    /// <summary>
    /// 菜单模块
    /// </summary>
    public class MenuModule : BaseModule
    {
        public override void Dispose()
        {
            
        }

        public void ShowMenu(bool isTween = false)
        {
            if (isTween)
            {
                var loading = UIWinMgr.Ins.Open<LoadingWin>();
                loading.onSwitch += () =>
                {                    
                    UIPanelMgr.Ins.Switch<SokobanMenuPanel>();
                };
            }
            else
            {                
                UIPanelMgr.Ins.Switch<SokobanMenuPanel>();
            }
        }


    }
}