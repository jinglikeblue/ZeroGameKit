using System;

namespace ZeroGameKit
{
    /// <summary>
    /// 信号接收者
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SignalReceiverAttribute : Attribute
    {
        public string SignalName { get; private set; }

        public SignalReceiverAttribute(string signalName)
        {
            if (string.IsNullOrEmpty(signalName))
            {
                throw new Exception("Wrong!");
            }

            this.SignalName = signalName;
        }
    }
}