namespace PingPong
{
    /// <summary>
    /// 网络延迟信息
    /// </summary>
    public class NetDelayInfo
    {
        /// <summary>
        /// 客户端到服务器的延迟
        /// </summary>
        public int c2s;
        
        /// <summary>
        /// 服务器到客户端的延迟
        /// </summary>
        public int s2c;
        
        /// <summary>
        /// 协议来回的延迟
        /// </summary>
        public int total;
    }
}