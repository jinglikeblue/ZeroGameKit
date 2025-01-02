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
    public class EventSystemHandler : MonoBehaviour
    {
        public static EventSystem EventSystem { get; private set; }
        public static StandaloneInputModule StandaloneInputModule { get; private set; }

        private void Awake()
        {
            EventSystem = GetComponent<EventSystem>();
            StandaloneInputModule = GetComponent<StandaloneInputModule>();

            DontDestroyOnLoad(this);
        }
    }
}