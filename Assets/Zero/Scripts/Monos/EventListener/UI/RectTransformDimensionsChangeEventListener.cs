using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    public class RectTransformDimensionsChangeEventListener : AEventListener<RectTransformDimensionsChangeEventListener>
    {
        public event Action onEvent;

        private void OnRectTransformDimensionsChange()
        {
            onEvent?.Invoke();
        }
    }
}
