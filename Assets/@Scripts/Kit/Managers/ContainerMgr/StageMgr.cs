using System;

namespace ZeroGameKit
{
    public class StageMgr : PanelContainerView
    {
        public static StageMgr Ins { get; private set; }
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
