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

        public static void Show()
        {
            if (null == _ins)
            {
                const string NAME = "GUIDeviceInfo";
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
            // 创建实际文本的GUIStyle，将颜色设置为白色
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontSize = 16;
            labelStyle.fontStyle = FontStyle.Bold;

            for (int i = 0; i < _infoItems.Count; i++)
            {
                var item = _infoItems[i];
                GUILayout.Label($"{item.key}:{item.value}", labelStyle);
                GUILayout.Space(-10);
            }
        }


    }
}