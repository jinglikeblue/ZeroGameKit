using System;

namespace ZeroGameKit
{
    public class UIPanelMgr : PanelContainerView
    {
        public static UIPanelMgr Ins { get; private set; }
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            if(null == Ins)
            {
                Ins = this;
            }
            else
            {
                throw new Exception("Inited");
            }
        }
    }
}
