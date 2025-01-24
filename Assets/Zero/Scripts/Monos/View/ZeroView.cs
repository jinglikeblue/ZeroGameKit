using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Zero
{
    // [HideMonoScript]
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

        [InlineButton("EditScript", "Edit"), DisplayAsString] [ShowIf("aViewObject")]
        [LabelText("AView子类名称")]
        [GUIColor("#50E3C2")]
        public string aViewClass;

        [Header("AView对象")] public object aViewObject;

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

#if UNITY_EDITOR

        /// <summary>
        /// 编辑与当前视图对象关联的脚本文件。
        /// 该方法会尝试查找并打开与视图对象类型名称匹配的脚本文件。
        /// </summary>
        void EditScript()
        {
            if (null == aViewObject) return;
            
            try
            {
                var scriptName = aViewObject.GetType().Name;
                // 在项目中查找类名对应的脚本文件
                string[] guids = UnityEditor.AssetDatabase.FindAssets($"{scriptName} t:Script");
                // 获取脚本的路径
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                // 打开该脚本文件
                UnityEditor.MonoScript script = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(assetPath);
                UnityEditor.AssetDatabase.OpenAsset(script);
            }
            catch (Exception e)
            {
                Debug.LogError($"无法打开脚本文件: {aViewClass}");
            }
        }

#endif
    }
}