using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    class GenerateAutoViewRegisterClassCommand : BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// 模板文件位置
        /// </summary>
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/ViewAutoRegisterTemplate.txt";

        /// <summary>
        /// 导出类位置
        /// </summary>
        public const string OUTPUT_FILE = "Assets/@Scripts/Generated/ViewAutoRegister.cs";
        //public const string OUTPUT_FILE = "ViewAutoRegister.cs";

        public const string SPECIFIC_R_FLAG = "[SPECIFIC R LIST]";

        public readonly List<AssetBundleItemVO> abList;

        /// <summary>
        /// 类文件模板
        /// </summary>
        string _classT;

        /// <summary>
        /// 注册视图调用模板
        /// </summary>
        string _registerT;

        /// <summary>
        /// 命名空间模板
        /// </summary>
        string _namespaceT;

        /// <summary>
        /// 明确指定type和prefab绑定的模板
        /// </summary>
        string _specificRegisterT;

        string[] _namespaceList;

        HashSet<string> _specificRegisteredSet = new HashSet<string>();

        public GenerateAutoViewRegisterClassCommand(List<AssetBundleItemVO> abList, string[] namespaceList)
        {
            _namespaceList = namespaceList;
            this.abList = abList;
        }

        public override void Excute()
        {
            _specificRegisteredSet.Clear();

            var dir = Directory.GetParent(OUTPUT_FILE);
            if (false == dir.Exists)
            {
                dir.Create();
            }

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _classT = template[0];
            _registerT = template[1].Replace("\r\n", "");
            _namespaceT = template[2].Replace("\r\n", "");
            _specificRegisterT = template[3].Replace("\r\n", "");

            string classContent = _classT.Replace(SPECIFIC_R_FLAG, GenerateSpecificItems());
            classContent = classContent.Replace(FIELD_LIST_FLAG, GenerateRegisterItems());
            classContent = classContent.Replace(PARAMS_FLAG, GenerateNamespaceList());
            File.WriteAllText(OUTPUT_FILE, classContent);
        }

        string GenerateSpecificItems()
        {
            StringBuilder sb = new StringBuilder();

            var aViewType = typeof(ZeroHot.AView);
            var assembly = Assembly.GetAssembly(aViewType);

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (false == type.IsSubclassOf(aViewType))
                {
                    continue;
                }

                var attrs = type.GetCustomAttributes(typeof(ViewRegisterAttribute), false);
                if (0 == attrs.Length)
                {
                    continue;
                }

                ViewRegisterAttribute attr = attrs[0] as ViewRegisterAttribute;

                string abName;
                string viewName;
                ResMgr.Ins.SeparateAssetPath(attr.prefabPath, out abName, out viewName);
                abName += ".ab";
                viewName = Path.GetFileNameWithoutExtension(viewName);
                //Debug.Log($"[ViewRegister] ab:{abName}  view:{viewName} type:{type.FullName}");

                var key = $"{abName}:{viewName}";
                _specificRegisteredSet.Add(key);

                sb.AppendLine(_specificRegisterT.Replace(FIELD_NAME_FLAG, abName).Replace(FIELD_VALUE_FLAG, viewName).Replace(TYPE_NAME_FLAG, type.FullName));
            }                    
            
            return sb.ToString();
        }

        string GenerateRegisterItems()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ab in abList)
            {
                foreach (var viewName in ab.assetList)
                {
                    if (Path.GetExtension(viewName).Equals(".prefab"))
                    {
                        var view = Path.GetFileNameWithoutExtension(viewName);

                        var key = $"{ab.assetbundle}:{view}";
                        if (false == _specificRegisteredSet.Contains(key))
                        {
                            sb.Append(_registerT.Replace(FIELD_NAME_FLAG, ab.assetbundle).Replace(FIELD_VALUE_FLAG, view));
                            sb.AppendLine();
                        }
                        else
                        {
                            //Debug.Log($"已明确注册的界面:{key}");
                        }
                    }
                }
            }            

            return sb.ToString();
        }

        string GenerateNamespaceList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ns in _namespaceList)
            {
                sb.Append(_namespaceT.Replace(NAMESPACE_FLAG, ns));
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
