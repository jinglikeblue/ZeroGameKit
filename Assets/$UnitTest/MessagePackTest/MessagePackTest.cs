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
        
        // var body = new Protocols.ProtocolBody();
        // body.id = 1;
        // body.data = new byte[0];
        // var a = MsgPacker.Pack(body);
        // var bodyC = MsgPacker.Unpack<Protocols.ProtocolBody>(a);

        var a = T2(typeof(FUCK));
        var b = T1<FUCK>();
    }

    struct FUCK
    {
        public int a;
    }

    object T1<T>()
    {
        return T2(typeof(T));
    }

    object T2(Type a)
    {
        return Activator.CreateInstance(a);
    }
}
