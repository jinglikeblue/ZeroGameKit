using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 指针从对象上抬起时的事件监听器
    /// </summary>
    public class PointerUpEventListener : AEventListener<PointerUpEventListener>, IPointerUpHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}