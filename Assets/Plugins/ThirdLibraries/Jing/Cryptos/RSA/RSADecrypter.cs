using System;
using System.Security.Cryptography;
using System.Text;

namespace Jing
{
    public class RSADecrypter : IDisposable
    {
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; private set; }

        private RSACryptoServiceProvider _provider;

        public RSADecrypter(string privateKey)
        {
            PrivateKey = privateKey;
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            _provider = rsa;
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data)
        {
            return _provider.Decrypt(data, false);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DecryptString(byte[] data)
        {
            byte[] decryptedBytes = _provider.Decrypt(data, false);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            _provider?.Dispose();
        }
    }
}