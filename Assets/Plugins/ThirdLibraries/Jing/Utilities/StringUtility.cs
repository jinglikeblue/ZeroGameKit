using System.Text;

namespace Jing
{
    public class StringUtility
    {
        /// <summary>
        /// 精简字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimUnusefulChar(string str)
        {
            char[] chars = str.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\0')
                {
                    str = new string(chars, 0, i);
                    break;
                }
            }

            return str;
        }

        /// <summary>
        /// 截取startFlag到endFlag之间的字符串
        /// </summary>
        /// <param name="input">要截取的字符串源</param>
        /// <param name="startFlag">作为开始标志的字符串</param>
        /// <param name="endFlag">作为结束标志的字符串</param>
        /// <param name="startIndex">查找开始的字符位置</param>
        /// <returns></returns>
        public static string Crop(string input, string startFlag, string endFlag, int startIndex = 0)
        {
            var cropStartIndex = input.IndexOf(startFlag, startIndex) + startFlag.Length;
            var cropEndIndex = input.IndexOf(endFlag, startIndex);
            return input.Substring(cropStartIndex, cropEndIndex - cropStartIndex);
        }

        /// <summary>
        /// 删除字符串中从startIndex开始的length个字符。并返回删除后的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Remove(string input, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new StringBuilder(input);
            sb.Remove(startIndex, length);
            return sb.ToString();
        }
    }
}