using Jing;
using PingPong;
using System.Reflection;
using UnityEngine;

public class MessagePackTest : MonoBehaviour
{
    struct Empty
    {
        public int i
        {
            get
            {
                return 1;
            }
            set
            {
                i = value;
            }
        }
    }
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
        
        
        var list = Protocols.GetProtocols();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
