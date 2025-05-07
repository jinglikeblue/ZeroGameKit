using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zero
{
    /// <summary>
    /// Unity场景中的EventSystem管理器
    /// </summary>
    [AddComponentMenu("Zero/Event/EventSystemHandler")]
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(StandaloneInputModule))]
    [DisallowMultipleComponent]
    public class EventSystemHandler :ASingletonMonoBehaviour<EventSystemHandler>
    {
        public static EventSystem EventSystem { get; private set; }
        public static StandaloneInputModule StandaloneInputModule { get; private set; }

        private void Awake()
        {
            Empty();
            EventSystem = GetComponent<EventSystem>();
            StandaloneInputModule = GetComponent<StandaloneInputModule>();
        }
    }
}