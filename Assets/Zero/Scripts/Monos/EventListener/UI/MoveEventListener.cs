using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// 指针移动时的事件监听器。
    /// 在发生移动事件（左、右、上、下）时调用   （这个是指↑↓←→来移动选择ui时触发的）。 作者：kd选帝侯 https://www.bilibili.com/read/cv26495444/ 出处：bilibili
    /// </summary>
    public class MoveEventListener : AEventListener<MoveEventListener>, IMoveHandler
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