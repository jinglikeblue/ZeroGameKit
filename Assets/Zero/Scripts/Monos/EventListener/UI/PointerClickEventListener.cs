using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 点击事件的监听器
    /// </summary>
    public class PointerClickEventListener : AEventListener<PointerClickEventListener>, IPointerClickHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}