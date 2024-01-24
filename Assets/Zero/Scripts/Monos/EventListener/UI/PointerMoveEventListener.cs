using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 指针移动时的事件监听器
    /// </summary>
    public class PointerMoveEventListener : AEventListener<PointerMoveEventListener>, IMoveHandler
    {
        public event Action<AxisEventData> onEvent;

        public void OnMove(AxisEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}