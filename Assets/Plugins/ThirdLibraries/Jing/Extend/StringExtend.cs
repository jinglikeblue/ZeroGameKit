using System;
using System.Text;

namespace Jing
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtend
    {
        /// <summary>
        /// 单个字符串替换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="ordinalIndex">要替换的字符串的出现索引（0表示第一个匹配的，1表示第二个匹配的）</param>
        /// <returns></returns>
        public static string ReplaceAt(this string input, string oldValue, string newValue, int ordinalIndex = 0)
        {
            if (ordinalIndex < 0)
            {
                return input;
            }

            int replaceStartIndex = -1;
            //首先找到对应的索引位
            while (--ordinalIndex >= -1)
            {
                replaceStartIndex = input.IndexOf(oldValue, replaceStartIndex + 1, StringComparison.Ordinal);
                if (-1 == replaceStartIndex)
                {
                    //找不到
                    return input;
                }
            }

            StringBuilder sb = new StringBuilder(input);
            sb.Remove(replaceStartIndex, oldValue.Length).Insert(replaceStartIndex, newValue);
            var output = sb.ToString();
            return output;
        }

        /// <summary>
        /// 移除传入的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="removeContent"></param>
        /// <param name="ordinalIndex">移除的字符串的出现索引（0表示第一个匹配的，1表示第二个匹配的）</param>
        /// <returns></returns>
        public static string RemoveAt(this string input, string removeContent, int ordinalIndex = 0)
        {
            return ReplaceAt(input, removeContent, string.Empty, ordinalIndex);
        }
    }
}