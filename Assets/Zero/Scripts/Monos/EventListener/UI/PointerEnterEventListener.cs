using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 监听点击进入事件的监听器
    /// </summary>
    public class PointerEnterEventListener : AEventListener<PointerEnterEventListener>, IPointerEnterHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}