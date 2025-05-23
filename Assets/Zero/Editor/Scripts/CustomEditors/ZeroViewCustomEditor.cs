using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zero;

namespace ZeroEditor
{
    [CustomEditor(typeof(ZeroView))]
    public class ZeroViewCustomEditor : OdinEditor
    {
        ZeroView Target => target as ZeroView;

        private string _prefabPath = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            _prefabPath = Target.PrefabPath != null ? Target.PrefabPath : TryFindPrefabPath();
        }

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

                if (_prefabPath != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Prefab");
                    if (EditorGUILayout.LinkButton($"{Path.GetFileNameWithoutExtension(_prefabPath)}"))
                    {
                        EditorUtility.FocusProjectWindow();
                        SelectionUtility.SelectAssetByGuid(AssetDatabase.AssetPathToGUID(_prefabPath));
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private string TryFindPrefabPath()
        {
            var gameObject = Target.gameObject;
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:Prefab {gameObject.name} ");
            List<string> assetPathList = new List<string>();
            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(assetPath) == gameObject.name)
                {
                    assetPathList.Add(assetPath);
                }
            }

            if (1 == assetPathList.Count)
            {
                return assetPathList[0];
            }

            return null;
        }
    }
}