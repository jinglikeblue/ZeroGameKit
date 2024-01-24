using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 拖拽结束事件的监听器
    /// </summary>
    public class PointerEndDragEventListener : AEventListener<PointerEndDragEventListener>, IEndDragHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnEndDrag(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}