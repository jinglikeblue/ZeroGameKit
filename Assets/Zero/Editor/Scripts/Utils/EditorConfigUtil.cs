using Jing;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ZeroEditor
{
    public class EditorConfigUtil
    {
        /// <summary>
        /// 编辑器生成的配置文件保存目录
        /// </summary>
        /// <returns></returns>
        static string ConfigDir
        {
            get
            {
                DirectoryInfo temp = Directory.GetParent(Application.dataPath);                
                if (!Directory.Exists(ZeroEditorConst.EDITOR_CONFIG_DIR))
                {
                    Directory.CreateDirectory(ZeroEditorConst.EDITOR_CONFIG_DIR);
                }
                return ZeroEditorConst.EDITOR_CONFIG_DIR;
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="data">配置的数据</param>
        /// <param name="fileName">文件名</param>
        public static void SaveConfig(object data, string fileName)
        {            
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(FileUtility.CombinePaths(ConfigDir, fileName), json, Encoding.UTF8);
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">配置文件名称</param>
        /// <returns></returns>
        public static T LoadConfig<T>(string fileName)
        {
            string path = FileUtility.CombinePaths(ConfigDir, fileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(json);
            }

            var obj = default(T);
            if(null == obj)
            {
                //不是struct是class,创建一个实例
                obj = System.Activator.CreateInstance<T>();
            }
            return obj;
        }
    }
}