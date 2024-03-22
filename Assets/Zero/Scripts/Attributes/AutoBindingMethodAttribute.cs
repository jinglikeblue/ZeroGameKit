using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Jing;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Zero
{
    public class AutoBindingMethodAttribute : Attribute
    {
    }

    /// <summary>
    /// 自动绑定 [UnityEngine.UI.Button的点击事件]
    /// </summary>
    public class AutoButtonClickBindingAttribute : Attribute
    {
        private const string START_FLAG = "On";
        private const string END_FLAG = "Click";

        /// <summary>
        /// 检查绑定，如果满足绑定条件，则自动进行绑定，并且返回对应的解绑方法列表。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<Action> Check(object obj)
        {
            List<Action> unbindingActionList = new List<Action>();

            var type = obj.GetType();
            var methodInfos = AttributeUtility.FindMethodInfos(type, typeof(AutoButtonClickBindingAttribute));
            foreach (var mi in methodInfos)
            {
                var fieldName = StringUtility.Crop(mi.Name, START_FLAG, END_FLAG);
                var fi = type.GetField(fieldName);
                if (fi == null)
                {
                    fi = type.GetField(char.ToLower(fieldName[0]) + fieldName.Substring(1));
                }

                if (null != fi)
                {
                    var button = (Button)fi.GetValue(obj);
                    var action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), obj, mi);
                    var unbindingAction = Binding(button, action);
                    unbindingActionList.Add(unbindingAction);
                }
            }

            return unbindingActionList;
        }

        static Action Binding(Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            return () => { button.onClick.RemoveListener(action); };
        }
    }
}