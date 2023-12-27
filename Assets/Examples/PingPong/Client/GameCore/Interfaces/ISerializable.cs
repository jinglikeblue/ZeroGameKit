namespace PingPong
{
    /// <summary>
    /// 可序列化接口
    /// </summary>
    public interface ISerializable<T>
    {
        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize();

        /// <summary>
        /// 返序列化化数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        void Deserialize(byte[] data);
    }
}
