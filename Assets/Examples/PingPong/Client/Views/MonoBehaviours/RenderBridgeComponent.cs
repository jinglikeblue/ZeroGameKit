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

        private void Update()
        {
            onRenderUpdate?.Invoke();
        }
    }
}
