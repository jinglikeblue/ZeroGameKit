using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jing
{
    /// <summary>
    /// AES加密处理类
    /// </summary>
    public class AesCryptoHandler
    {
        [UnityEditor.MenuItem("Test/CryptoFile/Encrypt")]
        public static void TestEncrypt()
        {
            var filePath = UnityEditor.EditorUtility.OpenFilePanel("test", "", "");
            var outputPath = FileUtility.StandardizeBackslashSeparator(filePath);
            outputPath = FileUtility.CombinePaths(Path.GetDirectoryName(outputPath), Path.GetFileName(outputPath) + ".crypto");
            var ach = new AesCryptoHandler(filePath, outputPath, "test");
            ach.Encrypt();
            Debug.Log("加密完成");
        }

        [UnityEditor.MenuItem("Test/CryptoFile/Decrypt")]
        public static void TestDecrypt()
        {
            var filePath = UnityEditor.EditorUtility.OpenFilePanel("test", "", "");
            var outputPath = FileUtility.StandardizeBackslashSeparator(filePath);
            outputPath = FileUtility.CombinePaths(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath));
            var ach = new AesCryptoHandler(filePath, outputPath, "test");
            ach.Decrypt();
            Debug.Log("解密完成");
        }

        public static byte[] GenerateKey(string key)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] key128Bit = new byte[16];
            var copyLength = keyBytes.Length < key128Bit.Length ? keyBytes.Length : key128Bit.Length;
            Array.Copy(keyBytes, 0, key128Bit, 0, copyLength);
            return key128Bit;
        }

        public static byte[] GenerateIV(string iv, SymmetricAlgorithm sa)
        {
            if (null == iv)
            {
                iv = "aes";
            }

            var tempBytes = Encoding.ASCII.GetBytes(iv);
            var ivBytes = new byte[sa.BlockSize >> 3];
            var copyLength = tempBytes.Length < ivBytes.Length ? tempBytes.Length : ivBytes.Length;
            Array.Copy(tempBytes, 0, ivBytes, 0, copyLength);
            return ivBytes;
        }

        public string key { get; private set; }
        public string iv { get; private set; }

        Stream _input;
        Stream _output;

        public string inputPath { get; private set; }
        public string outputPath { get; private set; }

        public AesCryptoHandler(string inputPath, string outputPath, string key, string iv = null)
        {
            _input = new FileStream(inputPath, FileMode.Open);
            _output = new FileStream(outputPath, FileMode.Create);
            this.key = key;
            this.iv = iv;
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        void Transform(bool isEncrypt)
        {
            _input.Seek(0, SeekOrigin.Begin);
            _output.Seek(0, SeekOrigin.Begin);
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = GenerateKey(key);
                aes.IV = GenerateIV(iv, aes);
                //aes.Padding = PaddingMode.None;

                Stream readStream;
                Stream writeStream;                

                if (isEncrypt)
                {
                    ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);                    
                    CryptoStream cryptoStream = new CryptoStream(_output, transform, CryptoStreamMode.Write);

                    readStream = _input;
                    writeStream = cryptoStream;
                }
                else
                {
                    ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
                    CryptoStream cryptoStream = new CryptoStream(_input, transform, CryptoStreamMode.Read);

                    readStream = cryptoStream;
                    writeStream = _output;
                }

                const int BUFFER_SIZE = 256;
                var buffer = new byte[BUFFER_SIZE];
                
                do 
                {
                    int count = readStream.Read(buffer, 0, BUFFER_SIZE);
                    if (count <= 0)
                    {
                        //读取完成
                        break;
                    }

                    writeStream.Write(buffer, 0, count);
                } while (true);

                writeStream.Flush();
                writeStream.Close();
            }

            _input.Close();
            _output.Close();
        }

        public void Encrypt()
        {
            Transform(true);

            var inputFile = new FileInfo(inputPath);
            var outputFile = new FileInfo(outputPath);
            Debug.Log($"加密数据: {inputFile.Length} => {outputFile.Length}");
        }

        public void Decrypt()
        {
            Transform(false);

            var inputFile = new FileInfo(inputPath);
            var outputFile = new FileInfo(outputPath);
            Debug.Log($"解密数据: {inputFile.Length} => {outputFile.Length}");
        }
    }
}
