using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    class MessagerExample
    {
        static public void Start()
        {
            new MessagerExample();
        }

        public static StringBuilder sb;


        public MessagerExample()
        {
            sb = new StringBuilder();

            sb.AppendLine("实例化消息派发器");
            var md = new MessageDispatcher<Type>();

            sb.AppendLine("注册消息接收器");
            md.RegisterReceiver<MessageReceiverA>(MessageA.Type);
            md.RegisterReceiver(MessageB.Type, typeof(MessageReceiverB));

            sb.AppendLine("派发消息B");
            md.DispatchMessage(MessageB.Type, new MessageB());

            sb.AppendLine("派发消息A");
            md.DispatchMessage(MessageA.Type, new MessageA());            

            var msg = MsgWin.Show("MessagerExample", sb.ToString());
            msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);
        }


        class MessageA 
        {
            public static readonly Type Type = typeof(MessageA);

            public string name = "A";
            public int age = 1;

            override public string ToString()
            {
                return $"MessageA [name={name} age={age}]";
            }
        }

        class MessageB
        {
            public static Type Type => typeof(MessageB);

            public string nickname = "B";
            public int value = 111;

            override public string ToString()
            {
                return $"MessageB [nickname={nickname} value={value}]";
            }
        }


        class MessageReceiverA : BaseMessageReceiver<MessageA>
        {
            protected override void OnReceive(MessageA m)
            {
                sb.AppendLine("MessageReceiverA:OnReceive " + LogColor.Zero1(m.ToString()));
            }
        }

        class MessageReceiverB : BaseMessageReceiver<MessageB>
        {
            protected override void OnReceive(MessageB m)
            {
                sb.AppendLine("MessageReceiverB:OnReceive " + LogColor.Zero1(m.ToString()));
            }
        }
    }

    
}
