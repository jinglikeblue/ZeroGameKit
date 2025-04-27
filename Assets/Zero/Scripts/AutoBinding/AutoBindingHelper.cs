using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zero;

namespace Zero
{
    internal static class AutoBindingHelper
    {
        private delegate List<Action> TryBindingDelegate(AView view);
        
        private static readonly List<TryBindingDelegate> TryBindingDelegates;
        
        static AutoBindingHelper()
        {
            TryBindingDelegates = new List<TryBindingDelegate>();
            var types = typeof(AutoBindingHelper).Assembly.GetTypes();
            var bindingAttrs = types.Where(t => t.IsSubclassOf(typeof(BaseAutoBindingAttribute)));
            foreach (var attr in bindingAttrs)
            {
                var tryBindingMethod = attr.GetMethod("TryBinding", BindingFlags.Static | BindingFlags.Public);
                if (tryBindingMethod != null)
                {
                    var tryBindingDelegate = tryBindingMethod.CreateDelegate(typeof(TryBindingDelegate), null) as TryBindingDelegate;
                    TryBindingDelegates.Add(tryBindingDelegate);
                }
            }
        }

        public static List<Action> TryBinding(AView view)
        {
            List<Action> unbindingActionList = new List<Action>();
            foreach (var d in TryBindingDelegates)
            {
                unbindingActionList.AddRange(d.Invoke(view));
            }
            return unbindingActionList;
        }
    }
}