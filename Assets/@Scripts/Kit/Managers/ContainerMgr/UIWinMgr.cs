﻿using System;
using UnityEngine;

namespace ZeroGameKit
{
    public class UIWinMgr : WindowsContainerView
    {
        public static UIWinMgr Ins { get; private set; }
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
