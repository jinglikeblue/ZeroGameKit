using System;
using System.IO;

namespace Jing
{
    /// <summary>
    /// 日志类，跨环境兼容。根据平台逐步添加
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Log日志保存的位置。如果为null，则表示没有日志文件。
        /// </summary>
        public string FilePath { get; private set; }
        public Log(string filePath = null)
        {
            if(null != filePath)
            {
                CreateLogFile(filePath);
            }
        }

        /// <summary>
        /// 设置Log文件地址
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void SetFilePath(string filePath)
        {
            CreateLogFile(filePath);
        }

        void CreateLogFile(string filePath)
        {
            FilePath = filePath;
            //FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.WriteLine()
            //File.AppendAllLines
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        public void I(object message)
        {
            WriteToFile(message);

#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(message);
#endif
            Console.WriteLine(message);
        }

        void WriteToFile(object message)
        {
            if(null == FilePath)
            {
                return;
            }
            var str = message.ToString();
            File.AppendAllLines(FilePath, new string[] { str });
        }
    }
}
