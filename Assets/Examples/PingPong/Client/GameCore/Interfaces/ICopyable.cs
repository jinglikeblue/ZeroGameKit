namespace PingPong
{
    /// <summary>
    /// 可拷贝数据接口
    /// </summary>
    public interface ICopyable<T>
    {
        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <returns></returns>
        T Clone();

        /// <summary>
        /// 数据拷贝致目标
        /// </summary>
        /// <param name="target"></param>
        void CopyTo(T target);

        /// <summary>
        /// 是用源的数据覆盖自身
        /// </summary>
        /// <param name="source"></param>
        void CopyFrom(T source);
    }
}
