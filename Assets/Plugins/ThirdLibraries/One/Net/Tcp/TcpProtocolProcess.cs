using System;
using System.Collections.Generic;

namespace One
{
    /// <summary>
    /// 协议处理器  
    /// 协议包结构为 | ushort：协议数据长度 | 协议数据 |
    /// </summary>
    internal class TcpProtocolProcess
    {
        /// <summary>
        /// 解包协议数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>使用的数据长度</returns>
        public int Unpack(byte[] buffer, int available, Action<byte[]> onReceiveData)
        {
            ByteArray ba = new ByteArray(buffer, available);
            int used = 0;
            while (ba.ReadEnableSize > ByteArray.USHORT_SIZE)
            {
                //获取协议数据长度
                ushort protocolSize = ba.ReadUShort();
                if (ba.ReadEnableSize >= protocolSize)
                {
                    byte[] protocolData = ba.ReadBytes(protocolSize);
                    onReceiveData?.Invoke(protocolData);
                    //记录使用协议长度
                    used += ByteArray.USHORT_SIZE + protocolData.Length;
                }
            }
            return used;
        }

        /// <summary>
        /// 将协议数据打包为可以直接发送的字节数组
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public byte[] Pack(byte[] protocolData)
        {
            ByteArray ba = new ByteArray(protocolData.Length + ByteArray.USHORT_SIZE);
            ushort dataSize = (ushort)protocolData.Length;
            //写入协议数据长度
            ba.Write(dataSize);
            //写入协议数据
            ba.Write(protocolData);
            return ba.Bytes;
        }
    }
}
