using System.Collections.Generic;
using System.IO;
using Jing;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Editor下的数据持久化工具，类似PlayerPrefs
    /// </summary>
    public static class ZeroEditorPrefs
    {
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public static string FilePath => FileUtility.CombinePaths(ZeroConst.ZERO_LIBRARY_DIR, "zero_editor_prefs_data.json");

        /// <summary>
        /// 数据
        /// </summary>
        private static Dictionary<string, string> _data = null;

        /*
        [UnityEditor.MenuItem("Test/ZeroEditorPrefs/Set")]
        private static void TestSet()
        {
            Set("int", 1);
            Set("float", 1.2f);
            Set("string", "fuck");
            var vo = new AssetBundleItemVO();
            vo.assetbundle = "123123123";
            Set("object", vo);
        }

        [UnityEditor.MenuItem("Test/ZeroEditorPrefs/Get")]
        private static void TestGet()
        {
            Debug.Log(Get<int>("int", 2));
            Debug.Log(Get<float>("float"));
            Debug.Log(Get<string>("string"));
            Debug.Log(Get<AssetBundleItemVO>("object"));
            Debug.Log(Get<string>("string1", "without"));
        }
        */

        static ZeroEditorPrefs()
        {
            if (File.Exists(FilePath))
            {
                var jsonStr = File.ReadAllText(FilePath);
                _data = Json.ToObject<Dictionary<string, string>>(jsonStr);
            }
            else
            {
                _data = new Dictionary<string, string>();
            }
        }

        public static void Set<T>(string key, T value)
        {
            _data[key] = Json.ToJson(value);
            Save();
        }

        public static T Get<T>(string key, T defaultValue = default)
        {
            if (_data.ContainsKey(key))
            {
                var jsonStr = _data[key];
                return Json.ToObject<T>(jsonStr);
            }

            return defaultValue;
        }

        private static void Save()
        {
            var jsonStr = Json.ToJsonIndented(_data);
            File.WriteAllText(FilePath, jsonStr);
        }
    }
}