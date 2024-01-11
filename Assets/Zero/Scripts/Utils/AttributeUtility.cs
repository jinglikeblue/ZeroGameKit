using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zero
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
    }
}
