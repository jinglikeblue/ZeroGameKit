using System.Security.Cryptography;
using System.Text;

namespace Jing
{
    /// <summary>
    /// RSA加解密辅助类(非对称加密)
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// 创建密钥对
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        public static void GenerateKeyPair(out string publicKey, out string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                publicKey = rsa.ToXmlString(false); // 获取公钥，参数false表示只导出公钥部分
                privateKey = rsa.ToXmlString(true); // 获取私钥，参数true表示导出包含私钥的完整密钥信息
            }
        }

        /// <summary>
        /// 创建加密器
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static RSAEncrypter CreateEncrypter(string publicKey)
        {
            return new RSAEncrypter(publicKey);
        }

        /// <summary>
        /// 创建解密器
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static RSADecrypter CreateDecrypter(string privateKey)
        {
            return new RSADecrypter(privateKey);
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string publicKey, byte[] data)
        {
            using var rsa = new RSAEncrypter(publicKey);
            return rsa.Encrypt(data);
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decrypt(string privateKey, byte[] data)
        {
            using var rsa = new RSADecrypter(privateKey);
            return rsa.Decrypt(data);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] EncryptString(string publicKey, string content)
        {
            using var rsa = new RSAEncrypter(publicKey);
            return rsa.EncryptString(content);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecryptString(string privateKey, byte[] data)
        {
            using var rsa = new RSADecrypter(privateKey);
            return rsa.DecryptString(data);
        }
    }
}