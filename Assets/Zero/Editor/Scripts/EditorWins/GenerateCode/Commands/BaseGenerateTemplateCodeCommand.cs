using System.Text.RegularExpressions;
using UnityEngine;

namespace ZeroEditor
{
    abstract class BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// 模板内容分隔符
        /// </summary>
        public const string TEMPLATE_SPLIT = "------------------------------Split--------------------------------";

        #region 替换标记
        public const string CLASS_NAME_FLAG = "[CLASS NAME]";
        public const string CLASS_LIST_FLAG = "[CLASS LIST]";
        public const string FIELD_LIST_FLAG = "[FIELD LIST]";
        public const string FIELD_NAME_FLAG = "[FIELD NAME]";
        public const string FIELD_VALUE_FLAG = "[FIELD VALUE]";
        public const string EXPLAIN_FLAG = "[EXPLAIN]";
        public const string NAMESPACE_FLAG = "[NAMESPACE]";
        public const string PARAMS_FLAG = "[PARAMS]";
        #endregion

        public abstract void Excute();

        /// <summary>
        /// 确保字段名称是合法的代码字段名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        static public string MakeFieldNameRightful(string fieldName)
        {
            fieldName = fieldName.Replace(' ', '_');
            var firstChar = fieldName[0];
            Regex regex = new Regex("[a-zA-Z_]");
            if (false == regex.IsMatch(firstChar.ToString()))
            {
                Debug.LogWarningFormat($"字段不是合法的(前缀已自动添加下划线): {fieldName}");                
                fieldName = "_" + fieldName;
            }

            if (fieldName.IndexOf('.') > -1)
            {
                Debug.LogWarningFormat($"字段不是合法的(已自动替换'.'为'_'): {fieldName}");                
                fieldName = fieldName.Replace('.', '_');
            }
            return fieldName;
        }
    }


}
