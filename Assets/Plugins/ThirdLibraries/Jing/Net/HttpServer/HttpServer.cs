using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Jing.Net
{
    /// <summary>
    /// Http服务
    /// </summary>
    public class HttpServer
    {
        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 是否运行中
        /// </summary>
        public bool IsRunning => _listener == null ? false : true;

        HttpListener _listener = null;

        Dictionary<string, Type> _routerTypeDict = null;

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="routerTypes">路由数组</param>
        public void Start(int port, Type[] routerTypes)
        {
            if (null != _listener)
            {
                throw new Exception($"HttpServer不能重复启动!");
            }

            if (false == SocketUtility.CheckPortUseable(port))
            {
                throw new Exception($"端口[{port}]被占用！无法启动服务!");
            }

            CreateRouterTypeDict(routerTypes);

            Port = port;
            _listener = new HttpListener();
            Task.Run(ListeningThread);
            HttpUtility.Print($"服务启动线程: {Thread.CurrentThread.ManagedThreadId}");
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="assemblies">Router所在的程序集，会自动反射获取BaseRouter的子类</param>
        public void Start(int port, Assembly[] assemblies)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (var assembly in assemblies)
            {
                var routerTypes = TypeUtility.FindSubclasses(assembly, typeof(BaseHttpRouter));
                types.UnionWith(routerTypes);
            }
            Start(port, types.ToArray());
        }

        void CreateRouterTypeDict(Type[] routerTypes)
        {
            if (null == routerTypes)
            {
                throw new Exception($"没有配置路由数据!不能为null!");
            }

            var baseRouterType = typeof(BaseHttpRouter);

            _routerTypeDict = new Dictionary<string, Type>();

            foreach (var routerType in routerTypes)
            {
                if (false == routerType.IsSubclassOf(baseRouterType))
                {
                    throw new Exception($"错误的RouterType: {routerType.FullName}");
                }

                var router = (BaseHttpRouter)Activator.CreateInstance(routerType);

                if (_routerTypeDict.ContainsKey(router.Route))
                {
                    throw new Exception($"重复的Route配置: {_routerTypeDict[router.Route].FullName} 与 {routerType.FullName}");
                }

                HttpUtility.Print($"注册路由: [{router.Route}] => {routerType.FullName} ");
                _routerTypeDict.Add(router.Route, routerType);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (null != _listener)
            {
                _routerTypeDict = null;
                _listener.Stop();
                _listener = null;
            }
        }

        string ProcessContext(HttpListenerContext context)
        {
            // 获取路由
            string route = context.Request.Url.AbsolutePath;

            var info = $"收到请求上下文:[Thread:{Thread.CurrentThread.ManagedThreadId}] [{context.Request.HttpMethod}] [RouteAndQuery:{context.Request.Url.PathAndQuery}]";

            //找路由
            if (_routerTypeDict.TryGetValue(route, out var routerType))
            {
                HttpUtility.Print(info, ConsoleColor.DarkGreen);
                var router = (BaseHttpRouter)Activator.CreateInstance(routerType);
                return router.Handle(context.Request);
            }

            HttpUtility.Print(info, ConsoleColor.DarkGray);
            return $"No Route: {context.Request.Url.AbsolutePath}";
        }

        void ListeningThread()
        {
            HttpUtility.Print($"服务占用线程: {Thread.CurrentThread.ManagedThreadId} 占用端口: {Port}");

            try
            {
                // 设置监听的URL
                _listener.Prefixes.Add($"http://*:{Port}/");
                // 开始监听
                _listener.Start();
            }
            catch (Exception e)
            {
                Log.E($"Http服务启动失败！");
                Log.E(e);
                _listener = null;
            }

            while (null != _listener)
            {
                // 接收到一个请求
                HttpListenerContext context = _listener.GetContext();

                //放到线程池中处理上下文
                Task.Run(() =>
                {
                    #region 请求处理
                    string responseContent;
                    try
                    {
                        responseContent = ProcessContext(context);
                    }
                    catch (Exception ex)
                    {
                        responseContent = ex.Message;
                    }
                    #endregion

                    #region 返回数据
                    try
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(responseContent);
                        // 设置响应头信息
                        context.Response.ContentType = "text/plain; charset=utf-8";
                        context.Response.ContentLength64 = buffer.Length;
                        // 发送响应内容
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception e)
                    {
                        HttpUtility.Print($"请求处理出错: [{context.Request.Url.AbsoluteUri}]", ConsoleColor.Red);
                        Log.E(e);
                    }
                    finally
                    {
                        // 关闭响应
                        context.Response.Close();
                    }
                    #endregion
                });
            }
        }
    }
}