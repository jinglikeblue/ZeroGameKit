using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        string[] _namespaceList;

        public GenerateAutoViewRegisterClassCommand(List<AssetBundleItemVO> abList, string[] namespaceList)
        {
            _namespaceList = namespaceList;
            this.abList = abList;
        }

        public override void Excute()
        {
            var dir = Directory.GetParent(OUTPUT_FILE);
            if (false == dir.Exists)
            {
                dir.Create();
            }

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _classT = template[0];
            _registerT = template[1].Replace("\r\n", "");
            _namespaceT = template[2].Replace("\r\n", "");

            string classContent = _classT.Replace(FIELD_LIST_FLAG, GenerateRegisterItems());
            classContent = classContent.Replace(PARAMS_FLAG, GenerateNamespaceList());            

            File.WriteAllText(OUTPUT_FILE, classContent);
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
                        sb.Append(_registerT.Replace(FIELD_NAME_FLAG, ab.assetbundle).Replace(FIELD_VALUE_FLAG, view));
                        sb.AppendLine();
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
