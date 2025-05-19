using System;
using System.Text;

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
        public const string TYPE_NAME_FLAG = "[TYPE NAME]";
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
            var input = fieldName;
            var sb = new StringBuilder(input.Length);

            // 第一个字符必须为字母或下划线
            if (!char.IsLetter(input[0]) && input[0] != '_')
            {
                if (char.IsDigit(input[0]))
                {
                    sb.Append('_');
                }
            }
            
            // sb.Append(input[0]);

            // 后续字符可以是字母、数字或下划线[4](@ref)
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('_');
                }
            }

            // 检查是否为C#关键字[1](@ref)
            fieldName = sb.ToString();
            if (IsKeyword(fieldName))
            {
                fieldName = "@" + fieldName;
            }

            return fieldName;

            bool IsKeyword(string word)
            {
                // C#关键字列表（部分示例）
                string[] keywords =
                {
                    "int", "float", "double", "string",
                    "class", "void", "if", "else", "while"
                };
                return Array.Exists(keywords, k => k.Equals(word, StringComparison.Ordinal));
            }
        }
    }
}