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
        private static readonly Vector3[] FourCorners = new Vector3[4]; // Graphic 四点位置
        private static readonly Vector3 LabelOffset = new Vector3(-5, 10, 0); // Tips 位置偏移

        private static readonly GUIStyle LabelStyle = new GUIStyle
            { fontSize = 13, alignment = TextAnchor.MiddleLeft, normal = { textColor = Color.white } }; // Tips 文本样式

        private const float Size = 5; // 按钮大小
        private static readonly Vector3 Offset = new Vector3(Size, -Size, 0); // 左上按钮显示位置偏移

        static bool m_enabled;

        public static bool Enabled
        {
            get => m_enabled;
            set
            {
                m_enabled = value;
                EditorPrefs.SetBool("GraphicSelectorEnable", value);
            }
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            m_enabled = EditorPrefs.GetBool("GraphicSelectorEnable");
            SceneView.duringSceneGui += OnSceneViewGUI;
        }

        private static void OnSceneViewGUI(SceneView sceneView)
        {
            if (!m_enabled)
            {
                return;
            }

            var mousePosition = Event.current.mousePosition;
            var ray = Camera.current.ScreenPointToRay(new Vector3(mousePosition.x, -mousePosition.y + Camera.current.pixelHeight));

            var currentPrefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            var allGraphics = currentPrefabStage != null ? currentPrefabStage.prefabContentsRoot.GetComponentsInChildren<Graphic>() : FindObjectsOfType<Graphic>();

            //开始绘制GUI 
            foreach (var g in allGraphics)
            {
                if (g.canvas == null)
                {
                    continue;
                }

                var scale = g.canvas.rootCanvas.transform.localScale.x;

                if (g.enabled && g.gameObject.activeInHierarchy)
                {
                    var color = Color.green;
                    Handles.color = color;
                    g.rectTransform.GetWorldCorners(FourCorners);

                    var buttonPosition = FourCorners[1] + Offset * scale;

                    if (Handles.Button(buttonPosition, Quaternion.identity, Size * scale, 0, Handles.RectangleHandleCap))
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

                    if (ray.origin.x < buttonPosition.x + Size * scale / 2
                        && ray.origin.x > buttonPosition.x - Size * scale / 2
                        && ray.origin.y < buttonPosition.y + Size * scale / 2
                        && ray.origin.y > buttonPosition.y - Size * scale / 2)
                    {
                        Handles.Label(ray.origin + LabelOffset * scale, g.gameObject.name, LabelStyle);
                    }
                }
            }
        }
    }
}