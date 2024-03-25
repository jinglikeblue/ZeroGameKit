using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Jing
{
    /// <summary>
    /// HTTP相关工具类
    /// </summary>
    public static class HttpUtility
    {
        /// <summary>
        /// 通过Query数据生成Key-Value字典
        /// </summary>
        /// <param name="queryContent"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CreateQueryDict(HttpListenerRequest request)
        {
            return request.QueryString.AllKeys.ToDictionary(k => k, k => request.QueryString[k]);
        }

        /// <summary>
        /// 通过Query数据生成Key-Value字典
        /// </summary>
        /// <param name="queryContent"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CreateQueryDict(string queryContent)
        {
            // 获取参数
            Dictionary<string, string> dict = null;
            try
            {
                if (queryContent.Trim() != string.Empty)
                {
                    dict = queryContent.TrimStart('?').Split('&').Select(q => q.Split('=')).ToDictionary(s => s[0], s => s[1]);
                }
            }
            catch
            {
                Print($"[Http] Query无法正确解析:{queryContent}", ConsoleColor.DarkRed);
                dict = new Dictionary<string, string>();
            }
            return dict;
        }

        /// <summary>
        /// 打印Http相关的信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="color"></param>
        public static void Print(string content, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            Log.I($"[Http] {content}", color);
        }
    }
}
