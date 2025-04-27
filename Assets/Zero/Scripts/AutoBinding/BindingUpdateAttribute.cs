using System;
using System.Collections.Generic;
using System.Reflection;
using Zero;

namespace Zero
{
    /// <summary>
    /// 自动绑定GameObject的Update
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BindingUpdateAttribute : BaseAutoBindingAttribute
    {
        /// <summary>
        /// 检查绑定，如果满足绑定条件，则自动进行绑定，并且返回对应的解绑方法列表。
        /// </summary>
        /// <param name="view"></param>
        /// <returns>返回事件对应的注销Action列表</returns>
        public static List<Action> TryBinding(AView view)
        {
            List<Action> unbindingActionList = BindingMethods<AView, BindingUpdateAttribute>(view, BindingMethod);
            return unbindingActionList;
        }

        /// <summary>
        /// 绑定方法
        /// </summary>
        /// <param name="view"></param>
        /// <param name="methodInfo"></param>
        /// <param name="attribute"></param>
        /// <returns>对应的解绑方法</returns>
        private static Action BindingMethod(AView obj, MethodInfo methodInfo, BindingUpdateAttribute attribute)
        {
            //构建方法的包装器
            Action action = () => { methodInfo.Invoke(obj, null); };
            //处理触发条件
            var component = ComponentUtil.AutoGet<UpdateEventListener>(obj.gameObject);
            component.onUpdate += action;
            //包装释放方法并返回
            return () => { component.onUpdate -= action; };
        }
    }
}