using System;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 自定义的Editor界面：ZeroViewCustomEditor
    /// </summary>
    public class ZeroView : MonoBehaviour
    {
        /// <summary>
        /// 是否已执行Awake
        /// </summary>
        public bool IsAwake { get; private set; } = false;

        /// <summary>
        /// 是否已执行Start
        /// </summary>
        public bool IsStart { get; private set; } = false;

        public event Action onEnable;
        public event Action onDisable;
        public event Action onDestroy;

        /// <summary>
        /// AView对象
        /// </summary>
        public object aViewObject;

        /// <summary>
        /// 如果GameObject是Prefab实例化来的，则保存Prefab路径
        /// </summary>
        public string PrefabPath { get; internal set; }

        private void Awake()
        {
            IsAwake = true;
        }

        private void Start()
        {
            IsStart = true;
        }

        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        private void OnDisable()
        {
            onDisable?.Invoke();
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();

            //清空没用的事件
            onEnable = null;
            onDisable = null;
            onDestroy = null;
        }
    }
}