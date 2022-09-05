using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace One
{
    public class Log
    {
        public enum ELogLevel
        {
            NONE,
            ERROR,
            WARNING,
            INFO,                                              
        }

        /// <summary>
        /// 日志打印等级
        /// </summary>
        public static ELogLevel logLevel = ELogLevel.INFO;

        public static void LogLine(object message)
        {
            Console.WriteLine("[Thread:{0}] [Date:{1}] {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="message"></param>
        public static void I(object message)
        {
            if(logLevel < ELogLevel.INFO)
            {
                return;
            }

            LogLine(message);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        public static void I(string format, params object[] args)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            LogLine(string.Format(format, args));            
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void CI(ConsoleColor color, object message)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            ColorInfo(color, message);
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void CI(ConsoleColor color, string format, params object[] args)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            ColorInfo(color, format, args);
        }


        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(object message)
        {
            if (logLevel < ELogLevel.WARNING)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkYellow, message);
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(string format, params object[] args)
        {
            if (logLevel < ELogLevel.WARNING)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkYellow, format, args);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(object message)
        {
            if (logLevel < ELogLevel.ERROR)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkRed, message);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(string format, params object[] args)
        {
            if (logLevel < ELogLevel.ERROR)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkRed, format, args);
        }



        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        private static void ColorInfo(ConsoleColor color, object message)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            LogLine(message);
            Console.ForegroundColor = old;
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        private static void ColorInfo(ConsoleColor color, string format, params object[] args)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            var s = string.Format(format, args);
            LogLine(s);
            Console.ForegroundColor = old;
        }
    }
}
