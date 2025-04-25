using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jing;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    class GenerateABClassCommand : BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// AB名称类模板文件位置
        /// </summary>
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/AssetBundleNameClassTemplate.txt";

        /// <summary>
        /// 导出类位置
        /// </summary>
        public const string OUTPUT_FILE = "Assets/@Scripts/Generated/AB.cs";

        string _mainClassT;
        string _explainT;
        string _classT;
        string _fieldT;
        string _dicAddT;

        public readonly List<AssetBundleItemVO> abList;

        private HashSet<string> _assetPathSet;

        public GenerateABClassCommand(List<AssetBundleItemVO> abList)
        {
            this.abList = abList;
        }

        public override void Excute()
        {
            _assetPathSet = new HashSet<string>();
            var dir = Directory.GetParent(OUTPUT_FILE);
            if (false == dir.Exists)
            {
                dir.Create();
            }

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _mainClassT = template[0];
            _explainT = template[1];
            _classT = template[2];
            _fieldT = template[3].TrimEnd();
            _dicAddT = template[4].TrimEnd();

            string classContent;
            var mainClassName = Path.GetFileNameWithoutExtension(OUTPUT_FILE);
            classContent = _mainClassT.Replace(CLASS_NAME_FLAG, mainClassName);
            classContent = classContent.Replace(CLASS_LIST_FLAG, GenerateClassList());
            classContent = classContent.Replace("[KEY VALUE LIST]", GenerateKeyValueList());
            classContent = classContent.Replace("[UNIQUE ASSET PATH]", GenerateUniqueAssetPath());

            File.WriteAllText(OUTPUT_FILE, classContent);
        }

        /// <summary>
        /// 对名称唯一的资源，生成可以直接定位的常量，用来快速定位资源
        /// </summary>
        /// <returns></returns>
        private string GenerateUniqueAssetPath()
        {
            //文件名使用次数记录表
            Dictionary<string, int> fileNameUsedCountDict = new Dictionary<string, int>();
            //文件名刀路径的映射表
            Dictionary<string, string> fileNameToPathDict = new Dictionary<string, string>();

            #region 找出每个文件名的使用次数

            foreach (var assetPath in _assetPathSet)
            {
                var fileName = Path.GetFileName(assetPath);
                fileNameToPathDict[fileName] = assetPath;
                if (!fileNameUsedCountDict.TryAdd(fileName, 1))
                {
                    fileNameUsedCountDict[fileName]++;
                }
            }

            #endregion
            
            const string tabStr = "\t";
            string template = StringUtility.Remove(_fieldT, _fieldT.IndexOf(tabStr, StringComparison.Ordinal), tabStr.Length);
            StringBuilder sb = new StringBuilder();
            foreach (var kv in fileNameUsedCountDict)
            {
                if (1 == kv.Value)
                {
                    var filed = GenerateFiled(kv.Key, fileNameToPathDict[kv.Key], template);
                    sb.Append(filed);
                }
                else
                {
                    Debug.LogWarning($"[AB] 文件名重复，无法生成直接定位：{kv.Key}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 创建视图的AssetBundle查找表（多个视图同名的话，则表中没有该视图的记录，因为不精确）
        /// </summary>
        string GenerateKeyValueList()
        {
            HashSet<string> repeatViewNameSet = new HashSet<string>();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var vo in abList)
            {
                foreach (var viewName in vo.assetList)
                {
                    var ext = Path.GetExtension(viewName);
                    if (ext.Equals(".prefab"))
                    {
                        string fieldName = Path.GetFileNameWithoutExtension(viewName);
                        if (dic.ContainsKey(fieldName))
                        {
                            repeatViewNameSet.Add(fieldName);
                            continue;
                        }

                        dic[fieldName] = vo.assetbundle;
                    }
                }
            }

            //剔除重名的view
            foreach (var viewName in repeatViewNameSet)
            {
                dic.Remove(viewName);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var kv in dic)
            {
                sb.Append(_dicAddT.Replace(FIELD_NAME_FLAG, kv.Key).Replace(FIELD_VALUE_FLAG, kv.Value));
            }

            return sb.ToString();
        }

        string GenerateClassList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var vo in abList)
            {
                if (0 == vo.assetList.Count)
                {
                    continue;
                }

                var classContent = GenerateClass(vo);
                sb.Append(classContent);
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        string GenerateClass(AssetBundleItemVO vo)
        {
            var fieldList = GenerateFieldList(vo.assetbundle, vo.assetList);

            var content = _classT;
            content = content.Replace(CLASS_NAME_FLAG, vo.GetFieldName());
            content = content.Replace(FIELD_LIST_FLAG, fieldList);
            if (string.IsNullOrEmpty(vo.explain))
            {
                content = content.Replace(EXPLAIN_FLAG, "");
            }
            else
            {
                content = content.Replace(EXPLAIN_FLAG, _explainT.Replace(EXPLAIN_FLAG, vo.explain));
            }

            return content;
        }

        string GenerateFieldList(string abName, List<string> viewNameList)
        {
            //因为Unity构建AssetBundle时，会将大写转换为小写，所以这里同样的做一个名字转换为小写的处理
            abName = abName.ToLower();

            var abNameWithoutExt = abName;
            if (abNameWithoutExt.EndsWith(ZeroConst.AB_EXTENSION))
            {
                abNameWithoutExt = abName.Substring(0, abName.Length - ZeroConst.AB_EXTENSION.Length);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateFiled("NAME", abName));
            sb.AppendLine();

            foreach (var viewName in viewNameList)
            {
                //var fieldName = Path.GetFileNameWithoutExtension(viewName);
                string fieldName = Path.GetFileNameWithoutExtension(viewName);
                var ext = Path.GetExtension(viewName);
                if (false == ext.Equals(".prefab"))
                {
                    fieldName = string.Format("{0}_{1}", fieldName, ext.Replace(".", ""));
                }

                sb.Append(GenerateFiled(fieldName, viewName));
                //添加全名
                var assetPath = ResMgr.Ins.LinkAssetPath(abNameWithoutExt, viewName);
                sb.Append(GenerateFiled(fieldName + "_assetPath", assetPath));

                if (!_assetPathSet.Add(assetPath))
                {
                    throw new Exception($"不应该出现重复的AssetPath: {assetPath} !!!");
                }
            }

            return sb.ToString();
        }

        string GenerateFiled(string fieldName, string fieldValue, string template = null)
        {
            if (null == template)
            {
                template = _fieldT;
            }

            fieldName = MakeFieldNameRightful(fieldName);
            return template.Replace(FIELD_NAME_FLAG, fieldName).Replace(FIELD_VALUE_FLAG, fieldValue);
        }
    }
}