using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace ZeroEditor
{
    /// <summary>
    /// Scene场景下，快捷选中GameObject对象的辅助工具
    /// </summary>
    public class GameObjectSelector : Editor
    {
        /// <summary>
        /// 四点位置
        /// </summary>
        private static readonly Vector3[] FourCorners = new Vector3[4];

        /// <summary>
        /// 文本样式
        /// </summary>
        private static readonly GUIStyle LabelStyle = new GUIStyle
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold, 
            alignment = TextAnchor.MiddleLeft, 
            normal = { textColor = Color.green },
        }; 

        private const float Size = 5; // 按钮大小
        private static readonly Vector3 Offset = new Vector3(Size, -Size, 0); // 左上按钮显示位置偏移

        private static HashSet<long> _posUsedSet = new HashSet<long>();
        private static Vector3 _tempLabelPos = Vector3.zero;

        static bool m_enabled;

        public static bool Enabled
        {
            get => m_enabled;
            set => m_enabled = value;
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            m_enabled = true;
            SceneView.duringSceneGui += OnSceneViewGUI;
        }

        private static void OnSceneViewGUI(SceneView sceneView)
        {
            if (!Enabled)
            {
                return;
            }

            var mousePosition = Event.current.mousePosition;
            var ray = Camera.current.ScreenPointToRay(new Vector3(mousePosition.x, -mousePosition.y + Camera.current.pixelHeight));

            var currentPrefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            var allGraphics = currentPrefabStage != null ? currentPrefabStage.prefabContentsRoot.GetComponentsInChildren<Graphic>() : FindObjectsOfType<Graphic>();

            _posUsedSet.Clear();
            
            //开始绘制GUI 
            foreach (var g in allGraphics)
            {
                if (g.canvas == null)
                {
                    continue;
                }

                if (false == g.enabled || false == g.gameObject.activeInHierarchy)
                {
                    continue;
                }

                var scale = g.canvas.rootCanvas.transform.localScale.x;
                var buttonSize = Size * scale;

                var color = Color.green;
                Handles.color = color;
                g.rectTransform.GetWorldCorners(FourCorners);

                var buttonPosition = FourCorners[1] + Offset * scale;
                var posX = (long)buttonPosition.x;
                var posY = (long)buttonPosition.y;
                long key = posX << 32 | posY;

                while (_posUsedSet.Contains(key))
                {
                    buttonPosition.y += (buttonSize * 2);
                    posX = (long)buttonPosition.x;
                    posY = (long)buttonPosition.y;
                    key = posX << 32 | posY;
                }

                _posUsedSet.Add(key);

                if (Handles.Button(buttonPosition, Quaternion.identity, buttonSize, 0, Handles.RectangleHandleCap))
                {
                    var current = Event.current;
                    if (current.control)
                    {
                        //判断点击时是否按下了Ctrl键
                        Debug.Log(HierarchyEditorUtility.GetNodePath(g.gameObject));
                    }
                    else
                    {
                        Selection.activeObject = g.gameObject;
                    }
                }

                if (ray.origin.x < buttonPosition.x + buttonSize
                    && ray.origin.x > buttonPosition.x - buttonSize
                    && ray.origin.y < buttonPosition.y + buttonSize
                    && ray.origin.y > buttonPosition.y - buttonSize)
                {
                    var fullName = g.gameObject.name; 
                    _tempLabelPos.x = buttonPosition.x + buttonSize + 1;
                    _tempLabelPos.y = buttonPosition.y;
                    Handles.Label(_tempLabelPos, fullName, LabelStyle);
                }
            }
        }
    }
}