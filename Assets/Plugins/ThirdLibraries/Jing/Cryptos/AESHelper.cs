using System;
using System.IO;
using System.Text;

namespace Jing
{
    /// <summary>
    /// AES加密辅助类(对称加密)
    /// </summary>
    public class AESHelper
    {
        #region 测试代码

        //[UnityEditor.MenuItem("Test/CryptoFile/AES_Encrypt")]
        //public static void TestEncrypt()
        //{
        //    var filePath = UnityEditor.EditorUtility.OpenFilePanel("test", "", "");
        //    var outputPath = FileUtility.StandardizeBackslashSeparator(filePath);
        //    outputPath = FileUtility.CombinePaths(Path.GetDirectoryName(outputPath), Path.GetFileName(outputPath) + ".crypto");

        //    var bytes = Encrypt(File.ReadAllBytes(filePath), "test");
        //    File.WriteAllBytes(outputPath, bytes);

        //    Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 加密完成!");

        //    UnityEngine.Application.OpenURL(Path.GetDirectoryName(outputPath));
        //}

        //[UnityEditor.MenuItem("Test/CryptoFile/AES_Decrypt")]
        //public static void TestDecrypt()
        //{
        //    var filePath = UnityEditor.EditorUtility.OpenFilePanel("test", "", "");
        //    var outputPath = FileUtility.StandardizeBackslashSeparator(filePath);
        //    outputPath = FileUtility.CombinePaths(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath));

        //    var bytes = Decrypt(File.ReadAllBytes(filePath), "test");
        //    File.WriteAllBytes(outputPath, bytes);

        //    Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}] 加密完成!");

        //    UnityEngine.Application.OpenURL(Path.GetDirectoryName(outputPath));
        //}

        #endregion


        static byte[] TrimBytes(byte[] input)
        {
            int outputSize = input.Length;
            //精简输出块
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\0')
                {
                    outputSize = i;
                    break;
                }
            }
            var output = new byte[outputSize];
            Array.Copy(input, 0, output, 0, outputSize);
            return output;
        }

        public static byte[] Encrypt(byte[] inputBytes, string key, string iv = null)
        {
            var input = new MemoryStream(inputBytes);
            var output = new MemoryStream();

            var handler = new AesCryptoHandler(input, output, key, iv);
            handler.Encrypt();
            var tempOutput = output.ToArray();
            return tempOutput;
        }

        public static byte[] Decrypt(byte[] inputBytes, string key, string iv = null)
        {
            var input = new MemoryStream(inputBytes);
            var output = new MemoryStream();

            var handler = new AesCryptoHandler(input, output, key, iv);
            handler.Decrypt();
            var tempOutput = output.ToArray();
            return tempOutput;
        }

        public static AesCryptoHandler EncryptAsync(Stream input, Stream output, string key, string iv = null)
        {
            var handler = new AesCryptoHandler(input, output, key, iv);
            handler.EncryptAsync();
            return handler;
        }

        public static AesCryptoHandler DecryptAsync(Stream input, Stream output, string key, string iv = null)
        {
            var handler = new AesCryptoHandler(input, output, key, iv);
            handler.DecryptAsync();
            return handler;
        }

        public static AesCryptoHandler EncryptAsync(string inputPath, string outputPath, string key, string iv = null)
        {
            var input = new FileStream(inputPath, FileMode.Open);
            var output = new FileStream(outputPath, FileMode.Create);
            var handler = EncryptAsync(input, output, key, iv);
            handler.onAsyncCompleted += () =>
            {
                input.Close();
                output.Close();
            };
            return handler;
        }

        public static AesCryptoHandler DecryptAsync(string inputPath, string outputPath, string key, string iv = null)
        {
            var input = new FileStream(inputPath, FileMode.Open);
            var output = new FileStream(outputPath, FileMode.Create);
            var handler = DecryptAsync(input, output, key, iv);
            handler.onAsyncCompleted += () =>
            {
                input.Close();
                output.Close();
            };
            return handler;
        }

        /// <summary>
        /// AES加密UTF8字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns>base64字符串</returns>
        public static string Encrypt(string input, string key, string iv = null)
        {
            var data = Encoding.UTF8.GetBytes(input);
            var encrypted = Encrypt(data, key, iv);
            var base64 = Convert.ToBase64String(encrypted);
            return base64;
        }

        /// <summary>
        /// AES解密UTF8字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Decrypt(string input, string key, string iv = null)
        {
            var encrypted = Convert.FromBase64String(input);
            var data = Decrypt(encrypted, key, iv);
            return Encoding.UTF8.GetString(data);
        }


    }
}
