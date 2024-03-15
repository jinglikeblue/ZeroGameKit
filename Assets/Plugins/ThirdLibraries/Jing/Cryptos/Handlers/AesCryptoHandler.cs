using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jing
{
    /// <summary>
    /// AES加密处理类
    /// </summary>
    public class AesCryptoHandler
    {
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

        public bool isDone { get; private set; } = false;

        public event Action onAsyncCompleted;

        public AesCryptoHandler(Stream input, Stream output, string key, string iv = null)
        {
            _input = input;
            _output = output;
            this.key = key;
            this.iv = iv;
        }

        void Transform(bool isEncrypt)
        {
            if (isDone)
            {
                throw new Exception("Transofrm is Done!");
            }

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
            }

            isDone = true;
        }

        public void Encrypt()
        {
            Transform(true);
        }

        public async void EncryptAsync()
        {
            await Task.Run(() => { Transform(true); });
            onAsyncCompleted?.Invoke();
        }

        public void Decrypt()
        {
            Transform(false);
        }

        public async void DecryptAsync()
        {
            await Task.Run(() => { Transform(false); });
            onAsyncCompleted?.Invoke();
        }
    }
}
