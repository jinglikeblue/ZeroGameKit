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
        DestroyImmediate(this);
        return;
#endif
        _camera = GetComponent<Camera>();
        _lastPriority = priority;
        Debug.Log($"Awake");
    }

    private void FixedUpdate()
    {
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
        _lastCameraEnable = _camera.enabled;
        //Debug.Log($"OnEnable {_camera.enabled}");
        URPCameraManager.UpdateCamera(this);
    }

    private void OnDisable()
    {
        //Debug.Log($"OnDisable {_camera.enabled}");
        URPCameraManager.UpdateCamera(this);
    }

    void OnCameraEnableChanged()
    {
        //Debug.Log($"Camera Enable Changed {_camera.enabled}");
        URPCameraManager.UpdateCamera(this);
    }
}