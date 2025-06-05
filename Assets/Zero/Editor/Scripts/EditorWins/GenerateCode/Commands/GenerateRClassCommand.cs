using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Jing;
using Sirenix.Utilities;
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
        public const string OUTPUT_FILE = ZeroEditorConst.HOT_SCRIPT_ROOT_DIR + "/Zero/Generated/R.cs";


        string _mainClassT;
        string _fieldT;
        List<string> _fieldNameList;

        public override void Excute()
        {
            _fieldNameList = new List<string>();

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _mainClassT = template[0];
            _mainClassT = _mainClassT.TrimEnd();
            _fieldT = template[1];

            string classContent;
            var mainClassName = Path.GetFileNameWithoutExtension(OUTPUT_FILE);
            classContent = _mainClassT.Replace(CLASS_NAME_FLAG, mainClassName);

            var rawFiles = FindHotFiles();
            FindAssetBundles(out var abFiles, out var assets);
            var allFiles = rawFiles.Concat(abFiles).Concat(assets).ToArray();
            var mapping = new FilePathMappingModel(allFiles);

            classContent = classContent.ReplaceAt(FIELD_LIST_FLAG, GenerateFieldList(rawFiles, mapping));
            classContent = classContent.ReplaceAt(FIELD_LIST_FLAG, GenerateFieldList(assets, mapping));
            classContent = classContent.ReplaceAt(FIELD_LIST_FLAG, GenerateFieldList(abFiles, mapping));

            File.WriteAllText(OUTPUT_FILE, classContent);
        }


        string GenerateFieldList(string[] files, FilePathMappingModel mapping)
        {
            files.Sort();
            StringBuilder sb = new StringBuilder();

            foreach (var path in files)
            {
                var fieldName = mapping.GetName(path);
                sb.Append(GenerateField(fieldName, path));
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
            var files = Directory.GetFiles(ZeroConst.PROJECT_FILES_DIR, "*", SearchOption.AllDirectories)
                .Where(file => !file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            foreach (var file in files)
            {
                var path = file; // file.Replace(ZeroConst.HOT_FILES_ROOT_DIR, ZeroConst.FILES_DIR_NAME);
                StripPath(ref path);

                path = FileUtility.StandardizeBackslashSeparator(path);
                list.Add(path);
            }

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

        public static AssetBundleItemVO[] FindAssetBundles(out string[] abFiles, out string[] assets)
        {
            List<string> fileList = new List<string>();
            List<string> assetList = new List<string>();

            var cmd = new FindAssetBundlesCommand(false);
            cmd.Excute();
            foreach (var item in cmd.list)
            {
                var abFile = FileUtility.CombinePaths(ZeroConst.PROJECT_AB_DIR, item.assetbundle);
                fileList.Add(abFile);

                var folder = abFile.Replace(".ab", "");
                StripPath(ref folder);

                foreach (var asset in item.assetList)
                {
                    var assetPath = FileUtility.CombinePaths(folder, asset);
                    assetList.Add(assetPath);
                }
            }

            // fileList.Sort();
            abFiles = fileList.ToArray();
            // assetList.Sort();
            assets = assetList.ToArray();


            return cmd.list.ToArray();
        }

        private static void StripPath(ref string path)
        {
            return;
            if (path.StartsWith(ZeroConst.PROJECT_ASSETS_DIR))
            {
                path = path.RemoveAt(ZeroConst.PROJECT_ASSETS_DIR, 0);
            }
        }
    }
}