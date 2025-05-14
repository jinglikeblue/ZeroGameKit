using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Jing;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 生成R.cs类
    /// </summary>
    class GenerateRClassCommand : BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// AB名称类模板文件位置
        /// </summary>
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/RClassTemplate.txt";

        /// <summary>
        /// 导出类位置
        /// </summary>
        public const string OUTPUT_FILE = "Assets/@Scripts/Generated/R.cs";


        string _mainClassT;
        string _fieldT;
        List<string> _fieldNameList;

        public override void Excute()
        {
            _fieldNameList = new List<string>();

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _mainClassT = template[0];
            _fieldT = template[1];

            string classContent;
            var mainClassName = Path.GetFileNameWithoutExtension(OUTPUT_FILE);
            classContent = _mainClassT.Replace(CLASS_NAME_FLAG, mainClassName);
            classContent = classContent.ReplaceAt(FIELD_LIST_FLAG, GenerateFieldList(FindHotFiles()));
            classContent = classContent.ReplaceAt(FIELD_LIST_FLAG, GenerateFieldList(FindAssetBundles()));

            File.WriteAllText(OUTPUT_FILE, classContent);
        }

        string GenerateFieldList(string[] files)
        {
            var mapping = new FilePathMappingModel(files);

            var keys = mapping.GetKeys();
            // Array.Sort(keys);

            StringBuilder sb = new StringBuilder();

            foreach (var key in keys)
            {
                var fieldName = key;
                var fieldValue = mapping.GetValue(key);
                sb.Append(GenerateField(fieldName, fieldValue));
            }

            return sb.ToString();
        }

        string GenerateField(string fieldName, string fieldValue)
        {
            fieldName = MakeFieldNameRightful(fieldName);

            _fieldNameList.Add(fieldName);

            return _fieldT.Replace(FIELD_NAME_FLAG, fieldName).Replace(FIELD_VALUE_FLAG, fieldValue);
        }

        /// <summary>
        /// 查找@Files目录下的所有文件，并返回对应的路径(路径格式和res.json匹配)
        /// </summary>
        /// <returns></returns>
        [UnityEditor.MenuItem("Test/FindHotFiles")]
        public static string[] FindHotFiles()
        {
            var list = new List<string>();
            var files = Directory.GetFiles(ZeroConst.HOT_FILES_ROOT_DIR, "*", SearchOption.AllDirectories)
                .Where(file => !file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            foreach (var file in files)
            {
                var path = file.Replace(ZeroConst.HOT_FILES_ROOT_DIR, ZeroConst.FILES_DIR_NAME);
                path = FileUtility.StandardizeBackslashSeparator(path);
                list.Add(path);
            }

            list.Sort();
            return list.ToArray();
        }

        /// <summary>
        /// 查找@Files目录下的所有文件，并返回对应的路径(路径格式和res.json匹配)
        /// </summary>
        /// <returns></returns>
        [UnityEditor.MenuItem("Test/R.cs")]
        public static void CreateRClass()
        {
            new GenerateRClassCommand().Excute();
        }

        public static string[] FindAssetBundles()
        {
            List<string> fileList = new List<string>();

            var cmd = new FindAssetBundlesCommand(false);
            cmd.Excute();
            foreach (var item in cmd.list)
            {
                var abFile = FileUtility.CombinePaths(ZeroConst.AB_DIR_NAME, item.assetbundle);
                fileList.Add(abFile);

                var folder = abFile.Replace(".ab", "");
                foreach (var asset in item.assetList)
                {
                    var assetPath = FileUtility.CombinePaths(folder, asset);
                    fileList.Add(assetPath);
                }
            }

            fileList.Sort();
            return fileList.ToArray();
        }
    }
}