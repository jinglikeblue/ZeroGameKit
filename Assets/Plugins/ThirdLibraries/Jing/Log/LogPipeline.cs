using System;
using System.IO;

namespace Jing
{
    /// <summary>
    /// 独立的日志管线。
    /// 可以实例化多条日志管线来打印日志以及写入到不同的日志文件中。
    /// </summary>
    public class LogPipeline
    {
        /// <summary>
        /// 日志信息等级。设置Log的logLevel，可以改变日志记录的类型
        /// </summary>
        public enum ELogLevel
        {
            /// <summary>
            /// 无Log
            /// </summary>
            NONE,

            /// <summary>
            /// 错误信息
            /// </summary>
            ERROR,

            /// <summary>
            /// 警告信息
            /// </summary>
            WARNING,

            /// <summary>
            /// 普通信息
            /// </summary>
            INFO,
        }

        /// <summary>
        /// 日志打印等级
        /// </summary>
        public ELogLevel logLevel = ELogLevel.INFO;

        /// <summary>
        /// Log日志保存的位置。如果为null，则表示没有日志文件。
        /// </summary>
        public string FilePath { get; private set; }
        public LogPipeline(string filePath = null)
        {
            SetFilePath(filePath);
        }

        /// <summary>
        /// 设置Log文件地址
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void SetFilePath(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// 普通信息
        /// </summary>
        /// <param name="message"></param>
        public void I(object message, ConsoleColor color = ConsoleColor.White)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            string logMsg = $"[I] {message}";
            Print(logMsg, color);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="message"></param>
        public void W(object message)
        {
            if (logLevel < ELogLevel.WARNING)
            {
                return;
            }

            string logMsg = $"[W] {message}";
            Print(logMsg, ConsoleColor.DarkYellow);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        public void E(object message)
        {
            if (logLevel < ELogLevel.ERROR)
            {
                return;
            }

            string logMsg = $"[E] {message}";
            Print(logMsg, ConsoleColor.DarkRed);
        }

        /// <summary>
        /// 打印日志信息
        /// </summary>
        /// <param name="logMsg"></param>
        void Print(string logMsg, ConsoleColor color = ConsoleColor.White)
        {
            WriteToFile(logMsg);

#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(logMsg);
#else
            ColorInfo(color, logMsg);
#endif
        }

        /// <summary>
        /// 日志写到文件中
        /// </summary>
        /// <param name="message"></param>
        void WriteToFile(object message)
        {
            if (null == FilePath)
            {
                return;
            }
            var str = message.ToString();
            File.AppendAllLines(FilePath, new string[] { str });
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="msg"></param>
        private void ColorInfo(ConsoleColor color, string msg)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = old;
        }
    }
}
