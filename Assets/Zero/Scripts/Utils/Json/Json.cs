using System;
using Newtonsoft.Json;

namespace  Zero
{
    /// <summary>
    /// JSON操作接口
    /// </summary>
    public static class Json
    {
        public static string ToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        
        public static string ToJsonIndented(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static object ToObject(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }
        
        public static object ToObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        public static T ToObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}

