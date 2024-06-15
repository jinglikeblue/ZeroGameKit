using System;
using System.Collections.Generic;
using System.Reflection;
using Jing;
using UnityEngine;
using UnityEngine.EventSystems;
using ZeroHot;

namespace Zero
{
    /// <summary>
    /// 自动绑定事件 [PointerClickEventListener]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BindingUIClickAttribute : Attribute
    {
        /// <summary>
        /// 绑定的节点的名称
        /// </summary>
        public string gameObjectName;

        /// <summary>
        /// 绑定的GameObject的名称。null表示绑定的为对象本身
        /// </summary>
        /// <param name="gameObjectName"></param>
        public BindingUIClickAttribute(string gameObjectName = null)
        {
            this.gameObjectName = gameObjectName;
        }

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
            var methodInfos = AttributeUtility.FindMethodInfos(type, typeof(BindingUIClickAttribute));
            foreach (var mi in methodInfos)
            {
                var attributes = mi.GetCustomAttributes<BindingUIClickAttribute>();
                foreach (var attribute in attributes)
                {
                    GameObject gameObject = null == attribute.gameObjectName
                        ? view.gameObject
                        : view.FindChildGameObject(attribute.gameObjectName);
                    var unbindingAction = Binding(gameObject,
                        (x) => { mi.Invoke(view, new object[] { gameObject }); });
                    unbindingActionList.Add(unbindingAction);
                }
            }

            return unbindingActionList.Count == 0 ? null : unbindingActionList;
        }

        static Action Binding(GameObject gameObject, Action<PointerEventData> action)
        {
            var component = ComponentUtil.AutoGet<PointerClickEventListener>(gameObject);
            component.onEvent += action;
            return () => { component.onEvent -= action; };
        }
    }
}