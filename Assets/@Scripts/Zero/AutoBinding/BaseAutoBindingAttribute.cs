using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zero
{
    /// <summary>
    /// 自动绑定标签的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class BaseAutoBindingAttribute : Attribute
    {
        /// <summary>
        /// 方法绑定处理器委托
        /// </summary>
        /// <typeparam name="TObj">目标实例类型</typeparam>
        /// <typeparam name="TAttribute">绑定标签类型</typeparam>
        /// <returns>返回的是对应的解绑方法</returns>
        protected delegate Action MethodBindingHandler<TObj, TAttribute>(TObj obj, MethodInfo mi, TAttribute attribute);

        /// <summary>
        /// 绑定方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bindingFunc"></param>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TObj"></typeparam>
        /// <returns></returns>
        protected static List<Action> BindingMethods<TObj, TAttribute>(TObj obj, MethodBindingHandler<TObj, TAttribute> bindingFunc) where TAttribute : Attribute
        {
            List<Action> unbindingActionList = new List<Action>();

            var type = obj.GetType();
            //TODO 这里可以做个缓存表用来查找type和方法的映射，提高效率
            var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var mi in methodInfos)
            {
                var attributes = mi.GetCustomAttributes<TAttribute>(true);
                foreach (var attribute in attributes)
                {
                    var releaseAction = bindingFunc.Invoke(obj, mi, attribute);
                    if (null != releaseAction)
                    {
                        unbindingActionList.Add(releaseAction);
                    }
                }
            }

            return unbindingActionList;
        }
    }
}