using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Jing
{
    /// <summary>
    /// 类工具
    /// </summary>
    public static class ClassUtility
    {
        /// <summary>
        /// 深度克隆对象
        /// 注意：慎用于性能敏感的场景
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeepClone<T>(T obj) where T : class
        {
            string xmlString = null;
// 序列化
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                xmlString = writer.ToString();
            }

            T cloneObj = null;
// 反序列化
            using (var reader = new StringReader(xmlString))
            {
                cloneObj = (T)serializer.Deserialize(reader);
            }

            return cloneObj;
        }

        /// <summary> 
        /// 将source对象的属性值复制到target对象中
        /// 注意：该操作为浅拷贝，引用类型的属性值拷贝的仍是引用
        /// 注意：慎用于性能敏感的场景
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="bindingAttr"></param>
        /// <typeparam name="T"></typeparam>
        public static void CopyTo<T>(T source, T target, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly) where T : class
        {
            if (null == source || null == target)
            {
                return;
            }

            PropertyInfo[] propertyInfos = typeof(T).GetProperties(bindingAttr);

            foreach (var pi in propertyInfos)
            {
                if (pi.CanWrite)
                {
                    object value = pi.GetValue(source);
                    pi.SetValue(target, value);
                }
            }

            FieldInfo[] filedInfos = typeof(T).GetFields(bindingAttr);

            foreach (var fi in filedInfos)
            {
                object value = fi.GetValue(source);
                fi.SetValue(target, value);
            }
        }
    }
}