using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour {

    [Header("相机观察对象")]
    public Transform lookTarget;
    [Header("相机最大距离")]
    public float maxDistance;
    [Header("相机最小距离")]
    public float minDistance;
    [Header("相机最大旋转速度")]
    public float maxRotaionSpeed;

    Vector3 _rotateDir;
    void Start () {
		
	}

    private void OnGUI()
    {
        
    }

    /// <summary>
    /// 移动摄像头
    /// </summary>
    /// <param name="dir"></param>
    public void rotate(Vector3 dir)
    {
        _rotateDir = dir;
        Rotate();
    }

    void Update () {

        Vector3 targetPos = lookTarget.position + Vector3.up;

        
        KeepDistance(targetPos);
        KeepLook(targetPos);
        
#if UNITY_EDITOR
        DebugDraw();
#endif
    }

    void Rotate()
    {
        transform.RotateAround(lookTarget.position, Vector3.up, maxRotaionSpeed * _rotateDir.x * Time.deltaTime);
    }

    /// <summary>
    /// 朝向角色
    /// </summary>
    /// <param name="targetPos"></param>
    void KeepLook(Vector3 targetPos)
    {        
        transform.LookAt(targetPos);
    }

    /// <summary>
    /// 保持距离
    /// </summary>
    /// <param name="targetPos"></param>
    void KeepDistance(Vector3 targetPos)
    {
        Vector2 cameraPos2 = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPos2 = new Vector2(targetPos.x, targetPos.z);

        Vector2 vectorFromTo = cameraPos2 - targetPos2;        
        if (vectorFromTo.magnitude > maxDistance)
        {            
            Vector2 v = vectorFromTo.normalized;
            v *= maxDistance;
            Vector2 newPos = targetPos2 + v;
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);            
        }   
        else if(vectorFromTo.magnitude < minDistance)
        {
            Vector2 v = vectorFromTo.normalized;
            v *= minDistance;
            Vector2 newPos = targetPos2 + v;
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);            
        }  
    }


    void DebugDraw()
    {
        Debug.DrawLine(transform.position, lookTarget.position + Vector3.up, Color.red);
    }

}
