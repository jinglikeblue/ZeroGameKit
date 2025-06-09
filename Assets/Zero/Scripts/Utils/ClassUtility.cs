using Jing;

namespace Zero
{
    /// <summary>
    /// 类工具
    /// </summary>
    public static class ClassUtility
    {
        /// <summary>
        /// 深度克隆对象
        /// 注意：性能偏低，慎用于性能敏感的场景
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeepClone<T>(T obj) where T : class
        {
            var tempBytes = MsgPacker.Pack(obj);
            return MsgPacker.Unpack<T>(tempBytes);
        }
    }
}