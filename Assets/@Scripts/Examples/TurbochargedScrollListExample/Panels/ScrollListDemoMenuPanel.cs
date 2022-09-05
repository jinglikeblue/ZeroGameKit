using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class ScrollListDemoMenuPanel : AView
    {
        public Button btnExit;

        public Button btnV;
        public Button btnH;
        public Button btnGrid;

        protected override void OnEnable()
        {
            base.OnEnable();

            btnExit.onClick.AddListener(() =>
            {
                UIPanelMgr.Ins.Switch<MenuPanel>();
            });

            btnV.onClick.AddListener(() =>
            {
                UIPanelMgr.Ins.Switch<VerticalScrollListDemoPanel>();
            });

            btnH.onClick.AddListener(() =>
            {
                UIPanelMgr.Ins.Switch<HorizontalScrollListDemoPanel>();
            });

            btnGrid.onClick.AddListener(() =>
            {
                UIPanelMgr.Ins.Switch<GridScrollListDemoPanel>();
            });
        }
    }
}
