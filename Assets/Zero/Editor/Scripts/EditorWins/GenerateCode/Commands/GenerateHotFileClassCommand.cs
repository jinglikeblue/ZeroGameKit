using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    [Obsolete("请使用R.cs相关的资源管理替代该功能")]
    class GenerateHotFileClassCommand : BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// AB名称类模板文件位置
        /// </summary>
        public const string TEMPLATE_FILE = "Assets/Zero/Editor/Configs/HotFileNameClassTemplate.txt";

        /// <summary>
        /// 导出类位置
        /// </summary>
        public const string OUTPUT_FILE = ZeroEditorConst.HOT_SCRIPT_ROOT_DIR + "/Zero/Generated/HotFiles.cs";        

        string _mainClassT;
        string _fieldT;
        string _fieldNameT;

        List<string> _fieldNameList;

        public override void Excute()
        {
            _fieldNameList = new List<string>();

            var template = File.ReadAllText(TEMPLATE_FILE).Split(new string[] { TEMPLATE_SPLIT }, StringSplitOptions.RemoveEmptyEntries);
            _mainClassT = template[0];            
            _fieldT = template[1];
            _fieldNameT = template[2];

            string classContent;
            var mainClassName = Path.GetFileNameWithoutExtension(OUTPUT_FILE);
            classContent = _mainClassT.Replace(CLASS_NAME_FLAG, mainClassName);
            classContent = classContent.Replace(FIELD_LIST_FLAG, GenerateFieldList());
            classContent = classContent.Replace(PARAMS_FLAG, GenerateParamsList());

            File.WriteAllText(OUTPUT_FILE, classContent);
        }

        string GenerateFieldList()
        {

            var fileList = Directory.GetFiles(ZeroConst.PROJECT_FILES_DIR, "*", SearchOption.AllDirectories);

            StringBuilder sb = new StringBuilder();

            foreach (var file in fileList)
            {
                var path = FileUtility.StandardizeBackslashSeparator(file);                                
                if(path.EndsWith(".meta"))
                {
                    continue;
                }

                //Debug.Log(path);

                var hotFilePath = FileUtility.GetRelativePath(ZeroConst.PROJECT_FILES_DIR, path);

                var fieldName = hotFilePath.Replace("/", "_").ToUpper();
                var fieldValue = hotFilePath;

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

        string GenerateParamsList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var filedName in _fieldNameList)
            {
                var codeStr = _fieldNameT.Replace(FIELD_NAME_FLAG, filedName);
                sb.Append(codeStr);
            }

            return sb.ToString();
        }
    }
}
