using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZeroGameKit
{
    /// <summary>
    /// 相机管理器
    /// </summary>
    public class CameraMgr
    {
        #region 单例模式
        public static CameraMgr Ins { get; private set; } = new CameraMgr();

        private CameraMgr()
        {

        }
        #endregion

        /// <summary>
        /// UI相机
        /// </summary>
        Camera _uiCamera;

        /// <summary>
        /// 获取UI相机
        /// </summary>
        /// <returns></returns>
        public Camera GetUICamera()
        {
            return _uiCamera;
        }

        /// <summary>
        /// 注册UI相机
        /// </summary>
        /// <param name="uiCamera"></param>
        public void RegisterUICamera(Camera uiCamera)
        {
            _uiCamera = uiCamera;
        }

        /// <summary>
        /// 相机查找表
        /// </summary>
        Dictionary<string, Camera> _cameraDic = new Dictionary<string, Camera>();

        /// <summary>
        /// 注册一个相机
        /// </summary>
        /// <param name="name">相机的名称</param>
        /// <param name="camera">相机对象</param>
        public void RegisterCamera(string name, Camera camera)
        {
            _cameraDic[name] = camera;
        }

        /// <summary>
        /// 撤销一个相机
        /// </summary>
        /// <param name="name">相机的名称</param>
        public void DeregisterCamera(string name)
        {
            _cameraDic.Remove(name);
        }

        /// <summary>
        /// 获取相机
        /// </summary>
        /// <param name="name">相机的名称</param>
        /// <returns></returns>
        public Camera GetCamera(string name)
        {
            if (false == _cameraDic.ContainsKey(name))
            {
                return null;
            }

            var camera = _cameraDic[name];
            if(null == camera)
            {
                _cameraDic.Remove(name);
                return null;
            }

            return camera;
        }
    }
}
