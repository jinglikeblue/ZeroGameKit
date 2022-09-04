using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace One
{
    /// <summary>
    /// WebSocket协议处理器
    /// </summary>
    public sealed class WebSocketProtocolProcess
    {
        /// <summary>
        /// 协议升级为WebSocket使用的GUID
        /// </summary>
        const string WEB_SOCKET_UPGRADE_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        /// <summary>
        /// 负载数据内容
        /// </summary>
        public enum EOpcode
        {
            /// <summary>
            /// 继续帧
            /// </summary>
            CONTINUE = 0,
            /// <summary>
            /// 文本帧
            /// </summary>
            TEXT = 1,
            /// <summary>
            /// 二进制帧
            /// </summary>
            BYTE = 2,
            /// <summary>
            /// 连接关闭
            /// </summary>
            CLOSE = 8,
            PING = 9,
            PONG = 10,
        }

        public int Unpack(byte[] buf, int available, Action<EOpcode, byte[]> onReceiveData)
        {
            ByteArray ba = new ByteArray(buf, available, false);
            int used = 0;
            Unpack(ba, ref used, onReceiveData);
            return used;
        }

        void Unpack(ByteArray ba, ref int used, Action<EOpcode, byte[]> onReceiveData)
        {
            if (ba.ReadEnableSize < 2 * ByteArray.BYTE_SIZE)
            {
                //数据存在半包问题
                return;
            }

            int startPos = ba.Pos;

            //获取第一个byte
            byte byte1 = ba.ReadByte();
            bool fin = (byte1 & 128) == 128 ? true : false;
            var rsv123 = (byte1 & 112);
            var opcode = (EOpcode)(byte1 & 15);

            //获取第二个byte
            byte byte2 = ba.ReadByte();
            bool mask = (byte2 & 128) == 128 ? true : false;
            var payloadLen = (byte2 & 127);

            int dataSize = 0;
            switch (payloadLen)
            {
                case 127:
                    if (ba.ReadEnableSize < 2 * ByteArray.ULONG_SIZE)
                    {
                        //数据存在半包问题
                        return;
                    }
                    dataSize = (int)ba.ReadULong();
                    break;
                case 126:
                    if (ba.ReadEnableSize < 2 * ByteArray.USHORT_SIZE)
                    {
                        //数据存在半包问题
                        return;
                    }
                    dataSize = ba.ReadUShort();
                    break;
                default:
                    dataSize = payloadLen;
                    break;
            }

            byte[] maskKeys = null;
            if (mask)
            {
                if (ba.ReadEnableSize < 4 * ByteArray.BYTE_SIZE)
                {
                    //数据存在半包问题
                    return;
                }

                maskKeys = new byte[4];
                for (int i = 0; i < maskKeys.Length; i++)
                {
                    maskKeys[i] = ba.ReadByte();
                }
            }

            switch (opcode)
            {
                case EOpcode.CONTINUE:
                case EOpcode.PING:
                case EOpcode.PONG:
                case EOpcode.CLOSE:                    
                    //使用率低，暂不处理这种情况
                    onReceiveData?.Invoke(opcode, null);
                    break;
                case EOpcode.TEXT:
                case EOpcode.BYTE:
                    if (dataSize > 0)
                    {
                        if (ba.ReadEnableSize < dataSize)
                        {
                            //数据存在半包问题
                            return;
                        }

                        byte[] payloadData = ba.ReadBytes(dataSize);
                        if (mask)
                        {
                            for (int i = 0; i < payloadData.Length; i++)
                            {
                                var maskKey = maskKeys[i % 4];
                                payloadData[i] = (byte)(payloadData[i] ^ maskKey);
                            }
                        }

                        onReceiveData?.Invoke(opcode, payloadData);                        
                    }
                    break;                                                                         
                default:
                    Log.E(string.Format("不可识别的WS协议:{0}", opcode));
                    return;
            }

            used += ba.Pos - startPos;

            Unpack(ba, ref used, onReceiveData);
        }

        public byte[] CreatePingFrame()
        {
            byte[] pingFrame = CreateDataFrame(null, false, true, WebSocketProtocolProcess.EOpcode.PING);
            return pingFrame;
        }

        public byte[] CreatePongFrame()
        {
            byte[] pongFrame = CreateDataFrame(null, false, true, WebSocketProtocolProcess.EOpcode.PONG);
            return pongFrame;
        }

        /// <summary>
        /// 将要发送的数据封装为WebSocket通信数据帧。
        /// 默认mask为0
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <param name="isMask">是否做掩码处理（默认为false)</param>
        /// <param name="isFin">是否是结束帧(默认为true)</param>
        /// <param name="opcode">操作码(默认为TEXT)</param>
        byte[] CreateDataFrame(byte[] data, bool isMask = false, bool isFin = true, EOpcode opcode = EOpcode.TEXT)
        {
            int bufferSize = 10;
            if (null != data)
            {
                bufferSize += data.Length;
            }

            ByteArray ba = new ByteArray(bufferSize, false);

            int b1 = 0;
            if (isFin)
            {
                b1 = b1 | 128;
            }
            b1 = b1 | (int)opcode;
            ba.Write((byte)b1);

            int b2 = 0;
            byte[] maskKeys = null;
            if (isMask)
            {
                b2 = b2 | 128;

                maskKeys = new byte[4];
                Random rand = new Random();
                for (int i = 0; i < maskKeys.Length; i++)
                {
                    maskKeys[i] = (byte)rand.Next();                    
                }

                for (int i = 0; i < data.Length; i++)
                {
                    var maskKey = maskKeys[i % 4];
                    data[i] = (byte)(data[i] ^ maskKey);
                }
            }

            if (data != null)
            {
                if (data.Length > 65535)
                {
                    ba.Write((byte)(b2 | 127));
                    ba.Write((long)data.Length);
                }
                else if (data.Length > 125)
                {
                    ba.Write((byte)(b2 | 126));
                    ba.Write((ushort)data.Length);
                }
                else
                {
                    ba.Write((byte)(b2 | data.Length));
                }

                if (isMask)
                {
                    for (int i = 0; i < maskKeys.Length; i++)
                    {
                        ba.Write(maskKeys[i]);
                    }
                }

                ba.Write(data);
            }
            else
            {
                ba.Write((byte)0);
            }
            return ba.GetAvailableBytes();
        }

        public byte[] Pack(byte[] data)
        {
            return CreateDataFrame(data);
        }

        /// <summary>
        /// 创建WebSocket升级的返回数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public byte[] CreateUpgradeResponse(string keyValue)
        {
            //生成升级协议确认KEY
            string responseValue = keyValue + WEB_SOCKET_UPGRADE_GUID;
            byte[] bytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(responseValue));
            string base64Value = Convert.ToBase64String(bytes);

            //构建升级回复协议
            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 101 Switching Protocols\r\n");
            builder.Append("Upgrade: websocket\r\n");
            builder.Append("Connection: Upgrade\r\n");
            builder.AppendFormat("Sec-WebSocket-Accept: {0}\r\n", base64Value);
            builder.Append("\r\n");
            string responseData = builder.ToString();

            byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);

            return responseBytes;
        }

        /// <summary>
        /// 创建WebSocket升级的请求数据
        /// </summary>
        /// <returns></returns>
        public byte[] CreateUpgradeRequest()
        {
            //生成升级协议确认KEY
            string requestValue = "One WebSocket";
            byte[] bytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(requestValue));
            string base64Value = Convert.ToBase64String(bytes);

            //构建升级回复协议
            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 101 Switching Protocols\r\n");
            builder.Append("Upgrade: websocket\r\n");
            builder.Append("Connection: Upgrade\r\n");
            builder.Append("Sec-WebSocket-Version: 13\r\n");
            builder.AppendFormat("Sec-WebSocket-Key: {0}\r\n", base64Value);
            builder.Append("\r\n");
            string requestData = builder.ToString();

            byte[] responseBytes = Encoding.ASCII.GetBytes(requestData);

            return responseBytes;
        }

    }
}
