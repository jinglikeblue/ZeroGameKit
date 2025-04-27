using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using Zero;

namespace Zero
{
    /// <summary>
    /// 自动绑定事件 [PointerClickEventListener]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BindingUIClickAttribute : BaseAutoBindingAttribute
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
            return BindingMethods<AView, BindingUIClickAttribute>(view, BindingMethod);
        }

        private static Action BindingMethod(AView obj, MethodInfo mi, BindingUIClickAttribute attribute)
        {
            //确定要绑定的GameObject对象
            GameObject gameObject = null == attribute.gameObjectName ? obj.gameObject : obj.FindChildGameObject(attribute.gameObjectName);

            if (null == gameObject)
            {
                Debug.LogError($"[自动绑定][失败] ({obj.gameObject.name}) 下找不到 ({attribute.gameObjectName}) ");
                return null;
            }

            //构建方法包装
            Action<PointerEventData> action = ped => { mi.Invoke(obj, null); };
            //注册监听
            var component = ComponentUtil.AutoGet<PointerClickEventListener>(gameObject);
            component.onEvent += action;
            //返回释放操作
            return () => { component.onEvent -= action; };
        }
    }
}