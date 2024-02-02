using System.Diagnostics;
using System.Text;

namespace Jing
{
    /// <summary>
    /// 进程工具类
    /// </summary>
    public static class ProcessUtility
    {
        /// <summary>
        /// 执行命令行代码
        /// </summary>
        /// <param name="cmd"></param>
        public static void RunCommandLine(string content)
        {
#if UNITY_EDITOR_WIN
            Log.I($"执行CMD命令: {content}");
            var process = new Process();
            process.StartInfo.FileName = $"cmd.exe";
            process.StartInfo.Arguments = $"/c {content}";
            //隐藏控制台窗口（可选）
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute  = false;
            //重定向输入、输出和错误流到自定义文本框或日志文件等位置（可选）
            // process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("GB2312");
            process.StartInfo.RedirectStandardOutput  = true;
            process.StartInfo.StandardErrorEncoding = process.StartInfo.StandardOutputEncoding;
            process.StartInfo.RedirectStandardError  = true;
            // 开始执行命令
            process.Start();
            // 获取命令执行结果
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            // 关闭进程
            process.WaitForExit();
            
            if (!string.IsNullOrEmpty(output.Trim()))
            {
                Log.I(output);    
            }
            if (!string.IsNullOrEmpty(error.Trim()))
            {
                Log.E(error);    
            }
#endif
        }
    }
}