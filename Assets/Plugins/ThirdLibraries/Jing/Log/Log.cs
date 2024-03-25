using System;

namespace Jing
{
    /// <summary>
    /// 日志类，跨环境兼容。根据平台逐步添加
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 创建一个独立的日志管线
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static LogPipeline CreatePipeline(string filePath = null)
        {
            return new LogPipeline(filePath);
        }        

        /// <summary>
        /// 单例
        /// </summary>
        static LogPipeline _ins = new LogPipeline();

        /// <summary>
        /// Log日志保存的位置。如果为null，则表示没有日志文件。
        /// </summary>
        public static string FilePath => _ins.FilePath;

        /// <summary>
        /// 设置Log文件地址
        /// </summary>
        /// <param name="filePath">设置为null则表示关闭本地日志写入</param>
        /// <returns></returns>
        public static void SetFilePath(string filePath)
        {
            _ins.SetFilePath(filePath);
        }

        /// <summary>
        /// 普通信息
        /// </summary>
        /// <param name="message"></param>
        public static void I(object message, ConsoleColor color = ConsoleColor.White)
        {
            _ins.I(message, color);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void W(object message)
        {
            _ins.W(message);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void E(object message)
        {
            _ins.E(message);
        }


    }
}
