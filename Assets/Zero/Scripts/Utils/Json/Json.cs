using System;
using Newtonsoft.Json;

namespace Zero
{
    /// <summary>
    /// JSON操作接口
    /// </summary>
    public static class Json
    {
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string ToJsonIndented(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static object ToObject(string jsonText)
        {
            return JsonConvert.DeserializeObject(jsonText);
        }

        public static object ToObject(string jsonText, Type type)
        {
            return JsonConvert.DeserializeObject(jsonText, type);
        }

        /// <summary>
        /// json字符串转换为对象
        /// </summary>
        /// <param name="jsonText"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToObject<T>(string jsonText)
        {
            return JsonConvert.DeserializeObject<T>(jsonText);
        }

        /// <summary>
        /// 从资源路径加载JSON对象
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadObject<T>(string path)
        {
            var jsonText = Res.Load<string>(path);
            return null == jsonText ? default : ToObject<T>(jsonText);
        }
    }
}