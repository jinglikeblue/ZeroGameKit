using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameKit
{
    public class Touchpad : MonoBehaviour, IDragHandler
    {
        /// <summary>
        /// 当滑动值改变时触发
        /// </summary>
        public event Action<Vector2> onValueChange;

        public void OnDrag(PointerEventData eventData)
        {            
            onValueChange?.Invoke(eventData.delta);
        }
    }
}
