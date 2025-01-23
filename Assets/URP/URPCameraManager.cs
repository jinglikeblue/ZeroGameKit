using Jing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;


/// <summary>
/// 管理器
/// </summary>
public static class URPCameraManager
{
    static HashSet<URPCamera> _urpCameraSet = new HashSet<URPCamera>();

    public static void UpdateCamera(URPCamera urpCamera)
    {
        if (null == urpCamera || null == urpCamera.gameObject)
        {
            return;
        }
        
        bool isCameraActive = null != urpCamera.Camera && urpCamera.Camera.isActiveAndEnabled;

        Debug.Log($"[URP Camera] {urpCamera.gameObject.name} use:{isCameraActive}");

        //筛选出要使用的相机集合
        if (isCameraActive)
        {
            if (false == _urpCameraSet.Contains(urpCamera))
            {
                _urpCameraSet.Add(urpCamera);
                RefreshCameraSettings();
            }
        }
        else
        {
            if (_urpCameraSet.Contains(urpCamera))
            {
                _urpCameraSet.Remove(urpCamera);
                RefreshCameraSettings();
            }
        }

        //对当前集合中的相机进行排序
    }

    public static void UpdateCameraPriority(URPCamera urpCamera)
    {
        if (urpCamera.Camera.isActiveAndEnabled && _urpCameraSet.Contains(urpCamera))
        {
            RefreshCameraSettings();
        }
    }

    /// <summary>
    /// 当集合改变时执行
    /// </summary>
    static void RefreshCameraSettings()
    {
        Debug.Log($"[URP Camera] 相机集合改变");

        var list = _urpCameraSet.ToList();
        list.Sort((x, y) => { return x.priority < y.priority ? -1 : 1; });

        Camera baseCamera = null;
        for (var i = 0; i < list.Count; i++)
        {
            var urpCamera = list[i];

            Debug.Log($"[URP Camera] 相机:{urpCamera.gameObject.name} 序列:{urpCamera.priority}");

            if (0 == i)
            {
                //Base相机
                baseCamera = urpCamera.Camera;
                baseCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
                baseCamera.GetUniversalAdditionalCameraData().cameraStack.Clear();
            }
            else
            {
                //Overlay相机
                urpCamera.Camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
                baseCamera.GetUniversalAdditionalCameraData().cameraStack.Add(urpCamera.Camera);
            }
        }
    }
}