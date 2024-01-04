using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedTest : MonoBehaviour
{
    public float speedX = 0;
    int speedVectorX = 1;

    public float speedY = 0;
    int speedVectorY = 1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {               
        var moveDistanceX = Time.deltaTime * speedX * speedVectorX;
        var moveDistanceY = Time.deltaTime * speedY * speedVectorY;
        var pos = transform.localPosition;  
        
        pos.x += moveDistanceX;
        if(pos.x > 8.5f)
        {
            pos.x = 8.5f;
            speedVectorX *= -1;
        }
        else if(pos.x < -8.5f)
        {
            pos.x = -8.5f;
            speedVectorX *= -1;
        }

        pos.z += moveDistanceY;
        if(pos.z > 11.5f)
        {
            pos.z = 11.5f;
            speedVectorY *= -1;
        }
        else if(pos.z < -11.5f)
        {
            pos.z = -11.5f;
            speedVectorY *= -1;
        }


        transform.localPosition = pos;
    }
}
