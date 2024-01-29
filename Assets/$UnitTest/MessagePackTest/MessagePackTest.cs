using Jing;
using PingPong;
using UnityEngine;

public class MessagePackTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //FrameInput fi = FrameInput.Default;
        //fi.playerInputs[0].moveSpeed = 3;

        //var data = MsgPackSerializer.Serialize(fi);
        //FrameInput fio = MsgPackSerializer.Deserialize<FrameInput>(data);

        

        Protocols.FrameInputNotify pfi = new Protocols.FrameInputNotify();
        pfi.frame = 2;
        pfi.inputs = new Protocols.InputRequest[2];
        pfi.inputs[1].moveDir = 1;

        var data = MsgPacker.Pack(pfi);

        Debug.Log($"data:{data.Length}");
        var pfio = MsgPacker.Unpack<Protocols.FrameInputNotify>(data);
        Debug.Log(LitJson.JsonMapper.ToPrettyJson(pfio));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
