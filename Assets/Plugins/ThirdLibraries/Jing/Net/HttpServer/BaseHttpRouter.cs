using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;

namespace Jing.Net
{
    /// <summary>
    /// 路由处理器基类
    /// </summary>
    public abstract class BaseHttpRouter
    {
        public string BaseRoute { get; private set; } = "";
        protected BaseHttpRouter(string rootRoute)
        {
            BaseRoute = "/" + rootRoute;
        }

        protected BaseHttpRouter()
        {
            BaseRoute = "";
        }

        /// <summary>
        /// 对应的路由
        /// </summary>
        public string Route
        {
            get
            {
                return $"{BaseRoute}/{SelfRoute()}";
            }
        }

        protected abstract string SelfRoute();

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract string Handle(HttpListenerRequest request);

    }
}
