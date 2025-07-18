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
    public static class EditorPrefs
    {
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public static string FilePath => FileUtility.CombinePaths(ZeroEditorConst.EDITOR_CACHE_DIR, "zero_editor_prefs_data.json");

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


        static EditorPrefs()
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

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void Set<T>(string key, T value)
        {
            _data[key] = Json.ToJson(value);
            Save();
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(string key, T defaultValue = default)
        {
            if (_data.ContainsKey(key))
            {
                var jsonStr = _data[key];
                return Json.ToObject<T>(jsonStr);
            }

            return defaultValue;
        }

        /// <summary>
        /// 删除指定数据
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteKey(string key)
        {
            if (_data.Remove(key))
            {
                Save();
            }
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public static void DeleteAll()
        {
            if (_data.Count > 0)
            {
                _data.Clear();
                Save();
            }
        }

        private static void Save()
        {
            var jsonStr = Json.ToJsonIndented(_data);
            FileUtility.CreateFolder(FilePath);
            File.WriteAllText(FilePath, jsonStr);
        }
    }
}