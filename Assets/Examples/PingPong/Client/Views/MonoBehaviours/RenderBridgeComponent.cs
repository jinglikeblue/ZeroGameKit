using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 渲染回调桥接组件
    /// </summary>
    public class RenderBridgeComponent : MonoBehaviour
    {
        /// <summary>
        /// 渲染更新
        /// </summary>
        public event Action onRenderUpdate;

        /// <summary>
        /// 被销毁
        /// </summary>
        public event Action onDestroy;

        private void Update()
        {
            onRenderUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }
}
