using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Jing
{
    /// <summary>
    /// 通过异或运算对数据加密(优点是文件大小不会改变)
    /// </summary>
    public class XORHelper
    {
        //[UnityEditor.MenuItem("Test/CryptoFile/XOR_Encrypt")]
        //public static void TestEncrypt()
        //{
        //    var filePath = UnityEditor.EditorUtility.OpenFilePanel("test", "", "");
        //    var outputPath = FileUtility.StandardizeBackslashSeparator(filePath);
        //    outputPath = FileUtility.CombinePaths(Path.GetDirectoryName(outputPath), Path.GetFileName(outputPath) + ".crypto");

        //    Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 加密开始1");
        //    var handler = XORHelper.TransformAsync(filePath, outputPath, "test");
        //    handler.onAsyncCompleted += () =>
        //    {
        //        Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 加密完成!");
        //    };
        //    Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 加密开始2");
        //}

        //[UnityEditor.MenuItem("Test/CryptoFile/XOR_Decrypt")]
        //public static void TestDecrypt()
        //{
        //    var filePath = UnityEditor.EditorUtility.OpenFilePanel("test", "", "");
        //    var outputPath = FileUtility.StandardizeBackslashSeparator(filePath);
        //    outputPath = FileUtility.CombinePaths(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath));
        //    var handler = XORHelper.TransformAsync(filePath, outputPath, "test");
        //    handler.onAsyncCompleted += () =>
        //    {
        //        Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 处理完成!");
        //    };
        //    Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 处理开始");
        //}

        public static byte[] Transform(byte[] input, string key)
        {
            var inputStream = new MemoryStream(input);
            var output = new byte[input.Length];
            var outputStream = new MemoryStream(output);
            var handler = new XORCryptoHandler(inputStream, outputStream, key);
            handler.Transform();
            inputStream.Close();
            outputStream.Close();
            return output;
        }

        public static XORCryptoHandler TransformAsync(Stream input, Stream output, string key)
        {
            var handler = new XORCryptoHandler(input, output, key);
            handler.TransformAsync();
            return handler;
        }

        public static XORCryptoHandler TransformAsync(string input, string output, string key)
        {
            var inputStream = new FileStream(input, FileMode.Open);
            var outputStream = new FileStream(output, FileMode.Create);

            var handler = new XORCryptoHandler(inputStream, outputStream, key);
            handler.onAsyncCompleted += () =>
            {
                inputStream.Close();
                outputStream.Close();
            };
            
            handler.TransformAsync();            
            return handler;
        }
    }
}
