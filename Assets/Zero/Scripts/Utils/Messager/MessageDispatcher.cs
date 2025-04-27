using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 消息发送者
    /// </summary>
    public class MessageDispatcher<TCode>
    {
        class ReceiverItem
        {
            public Type receiverType;
            public MethodInfo onReceiveMethodInfo;
            public Type messageType;
        }

        Dictionary<TCode, ReceiverItem> _receiverDic;

        public MessageDispatcher()
        {
            _receiverDic = new Dictionary<TCode, ReceiverItem>();
        }

        /// <summary>
        /// 注册接受者
        /// </summary>
        /// <typeparam name="TReceiver"></typeparam>
        /// <param name="code"></param>
        public void RegisterReceiver<TReceiver>(TCode code) where TReceiver : IMessageReceiver
        {
            var receiverType = typeof(TReceiver);
            RegisterReceiver(code, receiverType);
        }

        public void RegisterReceiver(TCode code, Type receiverType)
        {
            //查找OnReceive方法           
            var onReceiveMethod = receiverType.GetMethod("OnReceive", BindingFlags.NonPublic | BindingFlags.Instance);
            //获取消息实体类型
            var messageType = onReceiveMethod.GetParameters()[0];

            var item = new ReceiverItem();
            item.receiverType = receiverType;
            item.onReceiveMethodInfo = onReceiveMethod;
            item.messageType = messageType.ParameterType;

            _receiverDic[code] = item;
        }

        /// <summary>
        /// 注销接受者
        /// </summary>
        /// <param name="code"></param>
        public void UnregisterReceiver(TCode code)
        {
            if (_receiverDic.ContainsKey(code))
            {
                _receiverDic.Remove(code);
            }
        }

        /// <summary>
        /// 清理所有的注册信息
        /// </summary>
        public void ClearAllRegistered()
        {
            _receiverDic.Clear();
        }

        /// <summary>
        /// 派送消息。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EDispatchResult DispatchMessage(TCode code, object message)
        {
            if (_receiverDic.ContainsKey(code))
            {
                var item = _receiverDic[code];

                try
                {
                    var receiverObj = Activator.CreateInstance(item.receiverType);
                    if (item.messageType != message.GetType())
                    {
                        return EDispatchResult.WRONG_TYPE;
                    }
                    item.onReceiveMethodInfo.Invoke(receiverObj, new object[] { message });
                    return EDispatchResult.SUCCESS;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return EDispatchResult.RECEIVE_ERROR;
                }
            }
            else
            {
                return EDispatchResult.UNREGISTERED;
            }
        }
    }
}
