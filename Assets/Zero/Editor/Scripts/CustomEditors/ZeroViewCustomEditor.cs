using behaviac;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine.SceneManagement;
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
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Prefab");
                if (EditorGUILayout.LinkButton($"{Target.aViewObject.GetType().Name}"))
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets($"{Target.aViewObject.GetType().Name} t:Prefab");
                    Debug.Log($"TODO 定位到对应的预制件。 找到数量:{guids.Length}");
                    if (guids.Length == 1)
                    {
                        var guid = guids[0];
                        SelectionUtility.SelectAssetByGuid(guid);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}