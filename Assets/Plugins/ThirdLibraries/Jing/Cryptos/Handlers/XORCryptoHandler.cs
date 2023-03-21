using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing
{
    public class XORCryptoHandler
    {
        Stream _input;
        Stream _output;

        string _key;

        public bool isDone { get; private set; } = false;

        public event Action onAsyncCompleted;


        public XORCryptoHandler(Stream input, Stream output, string key)
        {
            _input = input;
            _output = output;
            _key = key;
        }

        public void Transform()
        {
            if (isDone)
            {
                throw new Exception("Transofrm is Done!");
            }

            isDone = false;

            byte[] xorKeys = _key.Length > 1024 ? Encoding.ASCII.GetBytes(MD5Helper.GetMD5(_key)) : Encoding.ASCII.GetBytes(_key);

            var bufferSize = xorKeys.Length > 256 ? xorKeys.Length : 256;

            var buffer = new byte[bufferSize];

            int count;
            while ((count = _input.Read(buffer, 0, bufferSize)) > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var o = buffer[i];
                    var key = xorKeys[i % xorKeys.Length];
                    buffer[i] = (byte)(o ^ key);
                }
                _output.Write(buffer, 0, count);
            }

            isDone = true;
        }

        public async void TransformAsync()
        {
            await Task.Run(Transform);
            onAsyncCompleted?.Invoke();
        }
    }
}
