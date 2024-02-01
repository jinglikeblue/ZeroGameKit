using Jing;
using PingPong;
using System;
using System.Reflection;
using UnityEngine;
using ZeroHot;

public class MessagePackTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //FrameInput fi = FrameInput.Default;
        //fi.playerInputs[0].moveSpeed = 3;

        //var data = MsgPackSerializer.Serialize(fi);
        //FrameInput fio = MsgPackSerializer.Deserialize<FrameInput>(data);

        //var a = MsgPacker.Pack(new Empty());
        //var empty = MsgPacker.Unpack<Empty>(a);


        //Protocols.FrameInputNotify pfi = new Protocols.FrameInputNotify();
        //pfi.frame = 2;
        //pfi.inputs = new Protocols.InputRequest[2];
        //pfi.inputs[1].moveDir = 1;

        //var data = MsgPacker.Pack(pfi);

        //Debug.Log($"data:{data.Length}");
        //var pfio = MsgPacker.Unpack(typeof(Protocols.FrameInputNotify),data);
        //var json = Newtonsoft.Json.JsonConvert.SerializeObject(pfio, Newtonsoft.Json.Formatting.Indented);
        //Debug.Log(json);


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
                            Debug.Log($"Found Receiver: [{id}] => {type.FullName}");
                            md.RegisterReceiver(id, type);

                        }
                    }
                }
            }
        }

        //测试Receiver
        var body = new Protocols.GameStartNotify();
        md.DispatchMessage(body.GetType().GetHashCode(), body);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
