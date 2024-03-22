using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jing
{
    /// <summary>
    /// C#「特性」工具类
    /// </summary>
    public static class AttributeUtility
    {
        /// <summary>
        /// 查找使用了指定特性的类
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Type[] FindClasses(Type attributeType)
        {
            List<Type> findedTypes = new List<Type>();

            // 获取应用中所有的程序集
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 遍历所有程序集
            foreach (Assembly assembly in assemblies)
            {
                var types = FindClasses(assembly, attributeType);
                findedTypes.AddRange(types);
            }

            return findedTypes.ToArray();
        }      

        static List<Type> FindClasses(Assembly assembly, Type attributeType)
        {
            List<Type> findedTypes = new List<Type>();

            // 获取程序集中所有类型
            Type[] types = assembly.GetTypes();

            // 遍历所有类型，查找使用了 MyAttribute 的类
            foreach (Type type in types)
            {
                if (type.GetCustomAttribute(attributeType, false) != null)
                {
                    findedTypes.Add(type);
                }
            }

            return findedTypes;
        }

        /// <summary>
        /// 找到目标类型中，所有使用了指定特性的方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<MethodInfo> FindMethodInfos(Type type, Type attributeType)
        {
            var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<MethodInfo> findedList = new List<MethodInfo>();
            foreach (var mi in methodInfos)
            {
                if (mi.GetCustomAttributes(attributeType, true).Length > 0)
                {
                    findedList.Add(mi);
                }
            }

            return findedList;
        }
    }
}
