using UnityEngine;

[RequireComponent(typeof(Camera))]
public class URPCamera : MonoBehaviour
{
    Camera _camera;

    public Camera Camera
    {
        get { return _camera; }
    }

    [Header("渲染层级(值越大越靠上)")] public int priority = 0;

    bool _lastCameraEnable;

    int _lastPriority = 0;

    private void Awake()
    {
#if UNITY_2020_1_OR_NEWER
        Destroy(this);
        return;
#endif
        _camera = GetComponent<Camera>();
        _lastPriority = priority;
        Debug.Log($"Awake");
    }

    private void FixedUpdate()
    {
        if (null == _camera)
        {
            Destroy(this);
            return;
        }
        
        if (_camera.enabled != _lastCameraEnable)
        {
            _lastCameraEnable = _camera.enabled;
            OnCameraEnableChanged();
            return;
        }

        if (_lastPriority != priority)
        {
            _lastPriority = priority;
            URPCameraManager.UpdateCameraPriority(this);
        }
    }

    private void OnEnable()
    {
        if (null == _camera)
        {
            return;
        }
        
        _lastCameraEnable = _camera.enabled;
        //Debug.Log($"OnEnable {_camera.enabled}");
        URPCameraManager.UpdateCamera(this);
    }

    private void OnDisable()
    {
        if (null == _camera)
        {
            return;
        }
        
        //Debug.Log($"OnDisable {_camera.enabled}");
        URPCameraManager.UpdateCamera(this);
    }

    void OnCameraEnableChanged()
    {
        if (null == _camera)
        {
            return;
        }
        
        //Debug.Log($"Camera Enable Changed {_camera.enabled}");
        URPCameraManager.UpdateCamera(this);
    }
}