using MsgPack.Serialization;
using System;
using System.Collections.Generic;

namespace Jing
{
    /// <summary>
    /// MessagePack库封装
    /// </summary>
    public static class MsgPacker
    {
        /// <summary>
        /// MessagePackSerializer序列化缓存
        /// </summary>
        static readonly Dictionary<Type, MessagePackSerializer> _serializerCacheDic = new Dictionary<Type, MessagePackSerializer>();

        static MessagePackSerializer GetSerializer(Type type)
        {
            if (_serializerCacheDic.ContainsKey(type))
            {
                return _serializerCacheDic[type];
            }
            var serializer = MessagePackSerializer.Get(type);
            _serializerCacheDic[type] = serializer;
            return serializer;
        }

        /// <summary>
        /// 清理生成的序列化工具缓存。可以释放一定的内存空间，但是下次构建序列化工具会占用CPU开销。
        /// 一般建议不用清理。
        /// </summary>
        public static void ClearCache()
        {
            _serializerCacheDic.Clear();
        }

        /// <summary>
        /// 打包数据对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Pack(object obj)
        {
            var serializer = GetSerializer(obj.GetType());
            var bytes = serializer.PackSingleObject(obj);
            return bytes;
        }

        /// <summary>
        /// 拆包数据对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object Unpack(Type type, byte[] data)
        {
            var serializer = GetSerializer(type);
            var obj = serializer.UnpackSingleObject(data);
            return obj;
        }

        /// <summary>
        /// 拆包数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Unpack<T>(byte[] data)
        {
            var obj = Unpack(typeof(T), data);
            if (null == obj)
            {
                return default(T);
            }
            return (T)obj;
        }
    }
}