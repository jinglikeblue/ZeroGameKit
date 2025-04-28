using System.Collections.Generic;
using System.Reflection;

namespace ZeroGameKit
{
    /// <summary>
    /// 信号功能。通过发送信号，可以触发通过SignalReceiverAttribute标记，关注了信号的方法
    /// </summary>
    public static class Signaler
    {
        private delegate void SignalReceiveDelegate();

        /// <summary>
        /// 信号名称到委托列表的映射
        /// </summary>
        private static readonly Dictionary<string, List<SignalReceiveDelegate>> SignalNameToDelegatesDict = new();

        /// <summary>
        /// 注册程序集。找到被SignalReceiverAttribute的静态方法
        /// </summary>
        /// <param name="assembly"></param>
        public static void Register(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var mi in methodInfos)
                {
                    var attrs = mi.GetCustomAttributes<SignalReceiverAttribute>();
                    foreach (var attr in attrs)
                    {
                        Register(attr.SignalName, mi);
                    }
                }
            }
        }

        /// <summary>
        /// 注册信号对应的方法
        /// </summary>
        /// <param name="signalName"></param>
        /// <param name="action"></param>
        public static void Register(string signalName, MethodInfo action)
        {
            Register(signalName, action.CreateDelegate(typeof(SignalReceiveDelegate), null) as SignalReceiveDelegate);
        }

        static void Register(string signalName, SignalReceiveDelegate action)
        {
            if (SignalNameToDelegatesDict.TryGetValue(signalName, out var list))
            {
                if (!list.Contains(action))
                {
                    list.Add(action);
                }
            }
            else
            {
                list = new List<SignalReceiveDelegate> { action };
                SignalNameToDelegatesDict.Add(signalName, list);
            }
        }

        /// <summary>
        /// 发送信号
        /// </summary>
        /// <param name="signalName"></param>
        public static void Send(string signalName)
        {
            if (SignalNameToDelegatesDict.TryGetValue(signalName, out var list))
            {
                foreach (var d in list)
                {
                    d.Invoke();
                }
            }
        }
    }
}