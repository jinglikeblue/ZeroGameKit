using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jing
{
    /// <summary>
    /// 类型工具类
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// 通过名称，找到Assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Assembly FindAssembly(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var assemblyName = assembly.GetName();
                if (assemblyName.Name.Equals(name))
                {
                    return assembly;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过基类查找他的派生类
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] FindSubclasses(Type baseType)
        {
            List<Type> findedTypes = new List<Type>();

            // 获取应用中所有的程序集
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 遍历所有程序集
            foreach (Assembly assembly in assemblies)
            {
                var types = FindSubclasses(assembly, baseType);
                findedTypes.AddRange(types);
            }

            return findedTypes.ToArray();
        }

        /// <summary>
        /// 通过基类查找他的派生类
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] FindSubclasses(Assembly assembly, Type baseType)
        {
            List<Type> findedTypes = new List<Type>();

            // 获取程序集中所有类型
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (type.IsSubclassOf(baseType))
                {
                    findedTypes.Add(type);
                }
            }

            return findedTypes.ToArray();
        }
    }
}