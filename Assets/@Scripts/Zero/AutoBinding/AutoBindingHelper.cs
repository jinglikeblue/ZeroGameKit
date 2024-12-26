using System;
using System.Collections.Generic;
using ZeroHot;

namespace Zero
{
    public static class AutoBindingHelper
    {
        public static List<Action> TryBinding(AView view)
        {
            List<Action> unbindingActionList = new List<Action>();
            unbindingActionList.AddRange(BindingUIClickAttribute.TryBinding(view));
            unbindingActionList.AddRange(BindingUpdateAttribute.TryBinding(view));
            unbindingActionList.AddRange(AutoButtonClickBindingAttribute.Check(view));
            return unbindingActionList;
        }
    }
}