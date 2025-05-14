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
        /// <param name="ordinalIndex">要替换的字符串的出现索引</param>
        /// <returns></returns>
        public static string ReplaceAt(this string input, string oldValue, string newValue, int ordinalIndex = 0)
        {
            if (ordinalIndex < 0)
            {
                return oldValue;
            }

            int replaceStartIndex = -1;
            //首先找到对应的索引位
            while (--ordinalIndex >= -1)
            {
                replaceStartIndex = input.IndexOf(oldValue, replaceStartIndex + 1, StringComparison.Ordinal);
                if (-1 == replaceStartIndex)
                {
                    //找不到
                    return oldValue;
                }
            }

            StringBuilder sb = new StringBuilder(input);
            sb.Remove(replaceStartIndex, oldValue.Length).Insert(replaceStartIndex, newValue);
            var output = sb.ToString();
            return output;
        }
    }
}