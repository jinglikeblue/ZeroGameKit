using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Jing
{
    /// <summary>
    /// AES加密辅助类(对称加密)
    /// </summary>
    public class AESHelper
    {
        static byte[] GenerateAESKey(string key)
        {
            key = MD5Helper.GetShortMD5(key);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] key128Bit = new byte[16];
            Array.Copy(keyBytes, 0, key128Bit, 0, keyBytes.Length);
            return key128Bit;
        }

        public static void Encrypt(Stream input, Stream output, string key, string iv = null)
        {

        }

        public static void Decrypt(Stream input, Stream output, string key, string iv = null)
        {

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

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] input, string key, string iv = null)
        {
            return new AesCrypto(input, key, iv).Encrypt();
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] input, string key, string iv = null)
        {
            return new AesCrypto(input, key, iv).Decrypt();
        }
    }


    public sealed class AesCrypto
    {
        public byte[] input { get; private set; }
        public string key { get; private set; }
        public string iv { get; private set; }

        public AesCrypto(byte[] input, string key, string iv = null)
        {
            if (input == null || input.Length <= 0)
            {
                throw new ArgumentNullException("input");
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            this.input = input;
            this.key = key;
            this.iv = iv;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <returns></returns>
        public byte[] Encrypt()
        {
            byte[] output;

            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = GenerateKey();
                aes.IV = GenerateIV(aes);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                output = encryptor.TransformFinalBlock(input, 0, input.Length);
            }

            return output;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <returns></returns>
        public byte[] Decrypt()
        {
            byte[] output;
            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = GenerateKey();
                aes.IV = GenerateIV(aes);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var tempOutput = decryptor.TransformFinalBlock(input, 0, input.Length);

                int outputSize = tempOutput.Length;
                //精简输出块
                for (int i = 0; i < tempOutput.Length; i++)
                {
                    if (tempOutput[i] == '\0')
                    {
                        outputSize = i;
                        break;
                    }
                }
                output = new byte[outputSize];
                Array.Copy(tempOutput, 0, output, 0, outputSize);
            }
            return output;
        }

        byte[] GenerateKey()
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] key128Bit = new byte[16];
            var copyLength = keyBytes.Length < key128Bit.Length ? keyBytes.Length : key128Bit.Length;
            Array.Copy(keyBytes, 0, key128Bit, 0, copyLength);
            return key128Bit;
        }

        byte[] GenerateIV(SymmetricAlgorithm sa)
        {
            if(null == iv)
            {
                iv = "aes";
            }

            var tempBytes = Encoding.ASCII.GetBytes(iv);
            var ivBytes = new byte[sa.BlockSize >> 3];
            var copyLength = tempBytes.Length < ivBytes.Length ? tempBytes.Length : ivBytes.Length;
            Array.Copy(tempBytes, 0, ivBytes, 0, copyLength);
            return ivBytes;
        }
    }
}
