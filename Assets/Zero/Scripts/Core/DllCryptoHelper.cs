using System.Text;
using Jing;

namespace Zero
{
    /// <summary>
    /// Dll加解密类
    /// </summary>
    public static class DllCryptoHelper
    {
        private const string CryptoKey = "zerogamekit";

        private static readonly byte[] CryptoFlagBytes;

        static DllCryptoHelper()
        {
            const string flag = "[Zero By Jing]";
            CryptoFlagBytes = Encoding.UTF8.GetBytes(flag);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] bytes)
        {
            if (CheckIsEncrypted(bytes))
            {
                return bytes;
            }

            var encryptedBytes = XORHelper.Transform(bytes, CryptoKey);
            var ba = new ByteArray(bytes.Length + CryptoFlagBytes.Length);
            ba.Write(CryptoFlagBytes);
            ba.Write(encryptedBytes);
            var outputBytes = ba.GetAvailableBytes();
            return outputBytes;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] TryDecrypt(byte[] bytes)
        {
            if (false == CheckIsEncrypted(bytes))
            {
                return bytes;
            }

            var ba = new ByteArray(bytes);
            ba.SetPos(CryptoFlagBytes.Length);
            var encryptedBytes = ba.ReadBytes(ba.ReadableSize);

            return XORHelper.Transform(encryptedBytes, CryptoKey);
        }

        /// <summary>
        /// 检查是否已加密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static bool CheckIsEncrypted(byte[] bytes)
        {
            var ba = new ByteArray(bytes);
            foreach (var b in CryptoFlagBytes)
            {
                if (false == ba.ReadByte().Equals(b))
                {
                    return false;
                }
            }

            return true;
        }
    }
}