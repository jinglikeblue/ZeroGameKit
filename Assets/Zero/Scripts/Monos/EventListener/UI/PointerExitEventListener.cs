using System;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 监听点击退出事件的监听器
    /// </summary>
    public class PointerExitEventListener : AEventListener<PointerExitEventListener>, IPointerExitHandler
    {
        public event Action<PointerEventData> onEvent;

        public void OnPointerExit(PointerEventData eventData)
        {
            if (null == onEvent)
            {
                return;
            }
            onEvent.Invoke(eventData);
        }
    }
}