using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class GUIDebugInfo : MonoBehaviour
    {
        class InfoItem
        {
            /// <summary>
            /// KEY
            /// </summary>
            public string key = null;

            /// <summary>
            /// 优先级(值越小越靠前)
            /// </summary>
            public int priority = -1;

            /// <summary>
            /// 值
            /// </summary>
            public object value;
        }

        static object _threadLocker = new object();

        public static GUIDebugInfo _ins;

        static Dictionary<string, InfoItem> _infoItemDic = new Dictionary<string, InfoItem>();
        static List<InfoItem> _infoItems = new List<InfoItem>();

        /// <summary>
        /// 文本颜色
        /// </summary>
        public static Color TextColor = Color.white;

        /// <summary>
        /// 文本文字大小
        /// </summary>
        public static int TextSize = 20;

        /// <summary>
        /// 是否允许文字描边
        /// </summary>
        public static bool IsOutlineEnable = true;

        public static void Show(int textSize)
        {
            TextSize = textSize;
            Show();
        }
        
        public static void Show(Color textColor, int textSize)
        {
            TextColor = textColor;
            TextSize = textSize;
            Show();
        }

        public static void Show()
        {
            if (null == _ins && Debug.unityLogger.logEnabled)
            {
                const string NAME = "GUIDebugInfo";
                GameObject go = new GameObject();
                go.name = NAME;
                _ins = go.AddComponent<GUIDebugInfo>();
                DontDestroyOnLoad(go);
            }
        }

        /// <summary>
        /// 设置要限制的信息
        /// </summary>
        /// <param name="key">信息的KEY</param>
        /// <param name="value">信息的值</param>
        /// <param name="priority">信息的排序优先级</param>
        public static void SetInfo(string key, object value, int priority = int.MaxValue)
        {
            lock (_threadLocker)
            {
                bool isNeedReorder = false;
                if (false == _infoItemDic.ContainsKey(key))
                {
                    //没有信息记录，则添加一个
                    _infoItemDic[key] = new InfoItem();
                    _infoItemDic[key].key = key;
                    isNeedReorder = true;
                }

                var item = _infoItemDic[key];
                item.value = value;
                if (item.priority != priority)
                {
                    item.priority = priority;
                    isNeedReorder = true;
                }

                if (isNeedReorder)
                {
                    _infoItems = _infoItemDic.Values.OrderBy(x => x.priority).ToList();
                }
            }
        }

        public static void CleanInfo(string key)
        {
            lock (_threadLocker)
            {
                if (false == _infoItemDic.ContainsKey(key))
                {
                    return;
                }

                var item = _infoItemDic[key];
                _infoItemDic.Remove(key);
                _infoItems.Remove(item);
            }
        }

        /// <summary>
        /// 清理所有信息
        /// </summary>
        public static void Clear()
        {
            lock (_threadLocker)
            {
                _infoItemDic.Clear();
                _infoItems.Clear();
            }
        }

        public static void Close()
        {
            if (null != _ins)
            {
                GameObject.Destroy(_ins.gameObject);
                _ins = null;
            }
        }

        int _frameCount = 0;
        float _cd = 1f;
        int _avgFps = 0;

        private void Update()
        {
            _frameCount++;
            _cd -= Time.unscaledDeltaTime;
            if (_cd <= 0f)
            {
                _avgFps = _frameCount;
                _frameCount = 0;
                _cd = 1f;

                SetInfo("FPS", _avgFps, int.MinValue);
            }
        }

        private void OnGUI()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = TextColor;
            labelStyle.fontSize = TextSize;
            labelStyle.fontStyle = FontStyle.Bold;

            InfoItem[] items = null;
            lock (_threadLocker)
            {
                items = _infoItems.ToArray();
            }

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                string text = $"{item.key}:{item.value}";

                // 四个方向的偏移（上下左右）
                Rect baseRect = GUILayoutUtility.GetRect(new GUIContent(text), labelStyle);
                
                if (IsOutlineEnable)
                {
                    // 描边效果：先绘制4次偏移的黑色文本，再绘制原始白色文本
                    GUIStyle outlineStyle = new GUIStyle(labelStyle);
                    outlineStyle.normal.textColor = Color.black; // 描边颜色

                    const int offset = 2;
                    GUI.Label(new Rect(baseRect.x - offset, baseRect.y, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x + offset, baseRect.y, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x, baseRect.y - offset, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x, baseRect.y + offset, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x + offset, baseRect.y + offset, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x + offset, baseRect.y - offset, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x - offset, baseRect.y + offset, baseRect.width, baseRect.height), text, outlineStyle);
                    GUI.Label(new Rect(baseRect.x - offset, baseRect.y - offset, baseRect.width, baseRect.height), text, outlineStyle);
                }

                // 原始文本
                GUI.Label(baseRect, text, labelStyle);

                GUILayout.Space(-10);
            }
        }
    }
}