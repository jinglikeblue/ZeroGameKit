using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zero;

namespace Zero
{
    /// <summary>
    /// 自动绑定事件 [Button点击事件]
    /// 对应方法：方法名(GameObject obj)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BindingButtonClickAttribute : BaseAutoBindingAttribute
    {
        /// <summary>
        /// 绑定的节点的名称
        /// </summary>
        public string gameObjectName;

        /// <summary>
        /// 绑定的GameObject的名称。null表示绑定的为对象本身
        /// </summary>
        /// <param name="gameObjectName"></param>
        public BindingButtonClickAttribute(string gameObjectName = null)
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
            return BindingMethods<AView, BindingButtonClickAttribute>(view, BindingMethod);
        }

        private static Action BindingMethod(AView obj, MethodInfo mi, BindingButtonClickAttribute attribute)
        {
            //确定要绑定的GameObject对象
            GameObject gameObject = null == attribute.gameObjectName ? obj.gameObject : obj.FindChildGameObject(attribute.gameObjectName);

            if (null == gameObject)
            {
                Debug.LogError($"[自动绑定][失败] ({obj.gameObject.name}) 下找不到 ({attribute.gameObjectName}) ");
                return null;
            }


            var button = gameObject.GetComponent<Button>();
            if (null == button)
            {
                Debug.LogError($"[自动绑定][失败]{obj.gameObject.name} 不存在Button组件!");
                return null;
            }

            //构建方法包装
            UnityAction action = () => { mi.Invoke(obj, null); };
            //注册监听
            button.onClick.AddListener(action);

            //返回释放操作
            return () => { button.onClick.RemoveListener(action); };
        }
    }
}