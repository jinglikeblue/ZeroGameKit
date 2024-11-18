using System;
using System.Security.Cryptography;
using System.Text;

namespace Jing
{
    /// <summary>
    /// 加密器
    /// </summary>
    public class RSAEncrypter : IDisposable
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; private set; }

        private RSACryptoServiceProvider _provider;

        public RSAEncrypter(string publicKey)
        {
            PublicKey = publicKey;
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            _provider = rsa;
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data)
        {
            return _provider.Encrypt(data, false);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] EncryptString(string content)
        {
            return _provider.Encrypt(Encoding.UTF8.GetBytes(content), false);
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