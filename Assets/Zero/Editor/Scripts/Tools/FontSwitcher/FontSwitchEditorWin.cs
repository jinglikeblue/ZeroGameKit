using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Zero;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ZeroEditor
{
    /// <summary>
    /// 字体切换编辑器窗口
    /// </summary>
    public class FontSwitchEditorWin : OdinEditorWindow
    {
        public static FontSwitchEditorWin Open()
        {
            var win = GetWindow<FontSwitchEditorWin>("字体替换");
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(800, 300);
            win.position = rect;
            return win;
        }

        [Title("设置")]
        [LabelText("替换所有字体")]
        public bool isSwitchAll = false;

        [LabelText("要替换的字体")]
        [DisableIf("isSwitchAll")]
        public Font sourceFont;

        [LabelText("要设置的字体")]
        public Font targetFont;

        [Title("批量替换")]
        [LabelText("限定文件夹")]
        [FolderPath(AbsolutePath = false, UseBackslashes = false)]
        public string folder = "Assets";

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        [Button("执行", ButtonSizes.Large)]
        void Run()
        {
            if (!EditorUtility.DisplayDialog("提示", "确认执行?", "确定", "取消"))
            {
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("替换中...", string.Empty, 0);

                //找到所有的预制件、场景
                Debug.Log($"[Zero][FontSwitcher] 开始执行。操作文件夹：{folder}");

                var isReplaced = ReplaceFontInPrefab();
                if (isReplaced)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                isReplaced = ReplaceFontInScene();
                if (isReplaced)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log("[Zero][FontSwitcher] 执行结束");
            }
        }

        [Title("单独替换")]
        [Button("替换当前场景中的字体", ButtonSizes.Large)]
        void ReplaceCurrentScene()
        {
            if (!EditorUtility.DisplayDialog("提示", "确认替换当前场景中的字体?", "确定", "取消"))
            {
                return;
            }

            ReplaceScene(SceneManager.GetActiveScene());
        }

        private bool ReplaceFontInPrefab()
        {
            var guids = AssetDatabase.FindAssets($"t:Prefab", new[] { folder });
            var totalProgress = (float)guids.Length;
            for (int i = 0; i < guids.Length; i++)
            {
                float progress = (i + 1) / totalProgress;
                var guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("替换中...", Path.GetFileName(path), progress);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var isReplaced = ReplaceFont(prefab);

                Log(path, isReplaced);
            }

            return guids.Length > 0;
        }

        private bool ReplaceFontInScene()
        {
            var currentActivieScene = SceneManager.GetActiveScene().path;

            bool isReplaced = false;
            var guids = AssetDatabase.FindAssets($"t:Scene", new[] { folder });
            var totalProgress = (float)guids.Length;
            for (int i = 0; i < guids.Length; i++)
            {
                float progress = (i + 1) / totalProgress;
                var guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("替换中...", Path.GetFileName(path), progress);

                Scene scene = EditorSceneManager.OpenScene(path);
                bool isSceneReplaced = ReplaceScene(scene);
                if (isSceneReplaced)
                {
                    isReplaced = true;
                }

                Log(path, isSceneReplaced);
            }

            EditorSceneManager.OpenScene(currentActivieScene);

            return isReplaced;
        }

        bool ReplaceScene(Scene scene)
        {
            bool isSceneReplaced = false;
            foreach (GameObject gameObject in scene.GetRootGameObjects())
            {
                if (ReplaceFont(gameObject))
                {
                    isSceneReplaced = true;
                }
            }

            if (isSceneReplaced)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            return isSceneReplaced;
        }

        private bool ReplaceFont(GameObject prefab)
        {
            bool isReplaced = false;
            var texts = prefab.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                if (isSwitchAll || text.font == sourceFont)
                {
                    if (text.font != targetFont)
                    {
                        // Undo.RecordObject(text, "Change Font");
                        text.font = targetFont;
                        isReplaced = true;
                        EditorUtility.SetDirty(text);
                    }
                }
            }

            return isReplaced;
        }

        void Log(string path, bool isReplaced)
        {
            string log = $"[Zero][FontSwitcher] 场景: {path}， 是否进行了字体替换：{(isReplaced ? LogColor.Zero1("是") + " [Replaced]" : LogColor.Zero2("否"))}";
            Debug.Log(log);
        }
    }
}