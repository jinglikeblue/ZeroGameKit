using System;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 拖拽开始事件监听器
    /// </summary>
    public class PointerBeginDragEventListener : AEventListener<PointerBeginDragEventListener>, IBeginDragHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}