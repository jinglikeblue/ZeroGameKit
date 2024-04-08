using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.Net
{
    /// <summary>
    /// 服务器接口
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// 启动服务
        /// </summary>        
        /// <param name="port">监听的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        void Start(int port, int bufferSize);

        /// <summary>
        /// 关闭服务
        /// </summary>
        void Close();

        /// <summary>
        /// 新的客户端进入的事件
        /// </summary>
        event ClientEnterEvent onClientEnter;

        /// <summary>
        /// 客户端退出的时间
        /// </summary>
        event ClientExitEvent onClientExit;
    }
}
