using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Zero
{
    [RequireComponent(typeof(CanvasScaler))]
    public class ZeroUICanvas : MonoBehaviour
    {        
        /// <summary>
        /// 设计宽度
        /// </summary>
        public float DesignWidth { get; private set; }

        /// <summary>
        /// 设计高度
        /// </summary>
        public float DesignHeight { get; private set; }

        /// <summary>
        /// 设计宽高比
        /// </summary>
        public float DesignAspectRatio { get; private set; }

        /// <summary>
        /// 屏幕宽高比
        /// </summary>
        public float ScreenAspectRatio { get; private set; }

        /// <summary>
        /// UI渲染宽度
        /// </summary>
        public float RenderWidth { get; private set; }

        /// <summary>
        /// UI渲染高度
        /// </summary>
        public float RenderHeight { get; private set; }

        private void Awake()
        {
            Refresh();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void Refresh()
        {            
            var cs = gameObject.GetComponent<CanvasScaler>();
            DesignWidth = cs.referenceResolution.x;
            DesignHeight = cs.referenceResolution.y;
            DesignAspectRatio = DesignWidth / DesignHeight;
            ScreenAspectRatio = (float)Screen.width / Screen.height;
            var rt = gameObject.GetComponent<RectTransform>();
            RenderWidth = rt.rect.width;
            RenderHeight = rt.rect.height;
        }
    }
}
