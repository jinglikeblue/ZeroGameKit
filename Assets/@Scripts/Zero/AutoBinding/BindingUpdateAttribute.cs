using System;
using System.Collections.Generic;
using Jing;
using UnityEngine;
using ZeroHot;

namespace Zero
{
    /// <summary>
    /// 自动绑定GameObject的Update
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BindingUpdateAttribute : Attribute
    {
        /// <summary>
        /// 检查绑定，如果满足绑定条件，则自动进行绑定，并且返回对应的解绑方法列表。
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static List<Action> TryBinding(AView view)
        {
            var type = view.GetType();

            List<Action> unbindingActionList = new List<Action>();

            //找到所有带标记的方法
            var methodInfos = AttributeUtility.FindMethodInfos(type, typeof(BindingUpdateAttribute));
            foreach (var mi in methodInfos)
            {
                Action action = () => { mi.Invoke(view, null); };

                var unbindingAction = Binding(view.gameObject, action);
                unbindingActionList.Add(unbindingAction);
            }

            return unbindingActionList.Count == 0 ? null : unbindingActionList;
        }

        static Action Binding(GameObject gameObject, Action action)
        {
            var component = ComponentUtil.AutoGet<UpdateEventListener>(gameObject);
            component.onUpdate += action;
            return () => { component.onUpdate -= action; };
        }
    }
}