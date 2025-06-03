using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        // CameraMgr.Ins.RegisterCamera("test", camera);
    }

    float time;
    // Update is called once per frame
    void Update()
    {
        // time += Time.deltaTime;
        // if(time > 1)
        // {
        //     if (transform.Find("Camera") == null)
        //     {
        //         var a = 1;
        //     }
        //
        //     var camera = CameraMgr.Ins.GetCamera("test");
        //
        //     if (null != camera)
        //     {
        //         Debug.Log($"{camera.name}");
        //     }
        //
        //     time = 0;
        // }
    }
}
