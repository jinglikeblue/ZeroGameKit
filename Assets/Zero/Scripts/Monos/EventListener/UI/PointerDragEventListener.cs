using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 拖拽操作的事件监听器
    /// </summary>
    public class PointerDragEventListener : AEventListener<PointerDragEventListener>, IDragHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnDrag(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}