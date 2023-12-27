namespace PingPong
{
    /// <summary>
    /// 可日志化接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 转换为日志数据
        /// </summary>
        /// <returns></returns>
        string ToLog();
    }
}
