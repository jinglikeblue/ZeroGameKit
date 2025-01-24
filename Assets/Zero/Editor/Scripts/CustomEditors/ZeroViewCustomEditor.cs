using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Zero;

namespace ZeroEditor
{
    [CustomEditor(typeof(ZeroView))]
    public class ZeroViewCustomEditor : OdinEditor
    {
        ZeroView Target => target as ZeroView;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Target.aViewObject != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Class");
                if (EditorGUILayout.LinkButton($"{Target.aViewObject.GetType().FullName}"))
                {
                    ZeroEditorUtility.EditScript(Target.aViewObject);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}