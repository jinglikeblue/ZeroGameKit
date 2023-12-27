using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PingPong
{
    /// <summary>
    /// 拷贝工具类
    /// </summary>
    public static class CopyUtility
    {
        static readonly Type TYPE_ILIST = typeof(IList);

        static readonly Type TYPE_IDICTIONARY = typeof(IDictionary);

        static readonly Type TYPE_ISET = typeof(HashSet<>);

        static T DeepCopyList<T>(T source)
        {
            Type type = source.GetType();
            var sourceList = source as IList;
            var copy = Activator.CreateInstance(type, sourceList.Count);
            var copyList = copy as IList;
            for (var i = 0; i < sourceList.Count; i++)
            {
                var value = sourceList[i];
                var copiedValue = DeepCopy(value);
                if (type.IsArray)
                {
                    copyList[i] = copiedValue;
                }
                else
                {
                    copyList.Add(copiedValue);
                }
            }
            return (T)copy;
        }

        static T DeepCopyDictionary<T>(T source)
        {
            Type type = source.GetType();
            var copy = Activator.CreateInstance(type);
            var sourceDic = source as IDictionary;
            var copyDic = copy as IDictionary;
            foreach (var key in sourceDic.Keys)
            {
                copyDic.Add(DeepCopy(key), DeepCopy(sourceDic[key]));
            }

            return (T)copy;
        }

        static T DeepCopySet<T>(T source)
        {
            Type type = source.GetType();
            var copy = Activator.CreateInstance(type);
            var addMethod = type.GetMethod("Add");
            var sourceSet = source as IEnumerable;
            var sourceEnumerator = sourceSet.GetEnumerator();
            object[] parameters = new object[1];
            while (sourceEnumerator.MoveNext())
            {
                var value = sourceEnumerator.Current;
                var copiedValue = DeepCopy(value);
                parameters[0] = copiedValue;
                addMethod.Invoke(copy, parameters);
            }
            return (T)copy;
        }

        /// <summary>
        /// 深度拷贝数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T source)
        {
            if (source == null)
            {
                return default(T);
            }

            Type type = source.GetType();

            if (type.IsPrimitive || type == typeof(string))
            {
                return source;
            }

            if (TYPE_ILIST.IsAssignableFrom(type))
            {
                return DeepCopyList(source);
            }

            if (TYPE_IDICTIONARY.IsAssignableFrom(type))
            {
                return DeepCopyDictionary(source);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == TYPE_ISET)
            {
                return DeepCopySet(source);
            }

            var copy = Activator.CreateInstance(type);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Console.WriteLine($"properties:{properties.Length}");
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    object value = property.GetValue(source);
                    var copiedValue = DeepCopy(value);
                    property.SetValue(copy, copiedValue);
                }
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            Console.WriteLine($"fields:{fields.Length}");
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(source);
                var copiedValue = DeepCopy(value);
                field.SetValue(copy, copiedValue);
            }

            return (T)copy;
        }
    }
}
