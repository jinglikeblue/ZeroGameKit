using Jing;
using System;
using System.Reflection;
using UnityEngine;
using Zero;
using Zero;

namespace PingPong
{
    public static class Protocols
    {
        public static Type ProtocolAttributeType => typeof(ProtocolAttribute);

        /// <summary>
        /// 协议派发器
        /// </summary>
        public static MessageDispatcher<int> dispatcher { get; private set; }

        /// <summary>
        /// 是否进行了初始化
        /// </summary>
        private static bool _inited = false;

        static Protocols()
        {
            Init();
        }

        public static void Init()
        {
            if (_inited)
            {
                return;
            }

            _inited = true;
            PerformanceAnalysis.BeginAnalysis("CreateMessageDispatcher");
            CreateProtocolMap();
            CreateMessageDispatcher();
            var cost = PerformanceAnalysis.EndAnalysis("CreateMessageDispatcher");
            Debug.Log($"PingPong 协议构建耗时: {cost}");
        }

        /// <summary>
        /// 创建消息派发器
        /// </summary>
        static void CreateMessageDispatcher()
        {
            var receiverInterfaceType = typeof(IMessageReceiver);

            MessageDispatcher<int> md = new MessageDispatcher<int>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (false == type.IsAbstract)
                {
                    if (receiverInterfaceType.IsAssignableFrom(type))
                    {
                        var genericArguments = type.BaseType.GetGenericArguments();
                        if (genericArguments.Length == 1)
                        {
                            var protocolStruct = genericArguments[0];
                            if (protocolStruct.GetCustomAttribute(Protocols.ProtocolAttributeType) != null)
                            {
                                var id = Protocols.GetProtocolId(protocolStruct);
                                Debug.Log($"注册协议接收器: [{id}] => {type.FullName}");
                                md.RegisterReceiver(id, type);
                            }
                        }
                    }
                }
            }

            dispatcher = md;

            //测试Receiver
            //var body = new Protocols.GameStartNotify();
            //md.DispatchMessage(body.GetHashCode(), body);
        }

        /// <summary>
        /// 协议特性标记
        /// </summary>
        [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
        public class ProtocolAttribute : Attribute
        {
            public byte Id { get; private set; }

            public ProtocolAttribute(byte id)
            {
                Id = id;
            }
        }

        /// <summary>
        /// 协议ID查找表
        /// </summary>
        static BidirectionalMap<byte, Type> _protocolMap;

        /// <summary>
        /// 创建协议表
        /// </summary>
        static void CreateProtocolMap()
        {
            _protocolMap = new BidirectionalMap<byte, Type>();
            var protocolAttributeType = typeof(ProtocolAttribute);
            var protocolsType = typeof(Protocols);
            foreach (var nestedType in protocolsType.GetNestedTypes())
            {
                if (false == nestedType.IsValueType)
                {
                    //不是结构体，肯定不是协议
                    continue;
                }

                var protocolAttr = nestedType.GetCustomAttribute<ProtocolAttribute>();
                if (protocolAttr != null)
                {
                    //是协议对象
                    //ID
                    var id = protocolAttr.Id;
                    _protocolMap.Set(id, nestedType);
                    // Debug.Log($"注册协议:[{id}]({nestedType.FullName})");
                }
            }
        }

        /// <summary>
        /// 获取协议
        /// </summary>
        /// <returns></returns>
        public static BidirectionalMap<byte, Type>.MappingItem[] GetProtocols()
        {
            return _protocolMap.GetMappings();
        }

        public static int GetProtocolId(Type protocolStructType)
        {
            return _protocolMap.Get(protocolStructType);
        }

        public static Type GetProtocolStructType(byte protocolId)
        {
            return _protocolMap.Get(protocolId);
        }

        /// <summary>
        /// 协议打包
        /// </summary>
        /// <returns></returns>
        public static byte[] Pack(object obj)
        {
            var body = new ProtocolBody();
            body.id = _protocolMap.Get(obj.GetType());
            body.data = MsgPacker.Pack(obj);
            
            Debug.Log(LogColor.Orange($"发送协议 [type:{obj.GetType().Name}] [size:{body.data.Length}]"));
            
            return MsgPacker.Pack(body);
        }

        /// <summary>
        /// 协议解包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object Unpack(byte[] data)
        {
            var body = MsgPacker.Unpack<ProtocolBody>(data);
            var type = _protocolMap.Get(body.id);
            if (null == type)
            {
                throw new Exception($"协议处理器不存在 [协议ID: {body.id}]");
            }

            var obj = MsgPacker.Unpack(type, body.data);
            return obj;
        }

        /// <summary>
        /// 协议解包并且派发
        /// </summary>
        /// <param name="data"></param>
        public static void UnpackAndDispatch(byte[] data)
        {
            var body = Unpack(data);
            var id = _protocolMap.Get(body.GetType());
            
            Debug.Log(LogColor.Green($"收到协议 [type:{body.GetType().Name}][size:{data.Length}]"));
            
            EDispatchResult dispatchResult = dispatcher.DispatchMessage(id, body);
            switch (dispatchResult)
            {
                case EDispatchResult.SUCCESS:
                    // Debug.Log($"网络协议派发：[{id}]({body.GetType().FullName})");
                    break;
                default:
                    Debug.LogError($"网络协议派发出现问题：[{id}]({dispatchResult})");
                    break;
            }
        }

        /// <summary>
        /// 协议体
        /// </summary>
        public struct ProtocolBody
        {
            /// <summary>
            /// 协议id
            /// </summary>
            public byte id;

            /// <summary>
            /// 协议数据
            /// </summary>
            public byte[] data;
        }

        #region Client To Server

        /// <summary>
        /// 加入主机
        /// </summary>
        [Protocol(100)]
        public struct JoinHostRequest
        {
        }

        /// <summary>
        /// 游戏准备好了
        /// </summary>
        [Protocol(101)]
        public struct GameReadyRequest
        {
        }

        /// <summary>
        /// 玩家输入
        /// </summary>
        [Protocol(102)]
        public struct InputRequest
        {
            public byte moveDir;
        }

        /// <summary>
        /// Ping请求
        /// </summary>
        [Protocol(103)]
        public struct PingC2S
        {
            public long clientUTC;
        }

        #endregion

        #region Server To Client

        /// <summary>
        /// 游戏开始
        /// </summary>
        [Protocol(200)]
        public struct GameStartNotify
        {
        }

        /// <summary>
        /// 帧输入数据同步
        /// </summary>
        [Protocol(201)]
        public struct FrameInputNotify
        {
            public int frame;
            public InputRequest[] inputs;
        }

        /// <summary>
        /// Pong回复
        /// </summary>
        [Protocol(202)]
        public struct PongS2C
        {
            public long clientUTC;
            public long serverUTC;
        }

        #endregion
    }
}