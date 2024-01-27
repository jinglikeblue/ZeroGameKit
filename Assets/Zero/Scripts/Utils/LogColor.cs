namespace Zero
{
    /// <summary>
    /// 日志的颜色控制
    /// </summary>
    public static class LogColor
    {
        /// <summary>
        /// Zero框架专用配色
        /// </summary>
        const string COLOR_ZERO1_PRO = "50E3C2";
        const string COLOR_ZERO2_PRO = "D6E884";
        const string COLOR_ZERO1 = "622C8E";
        const string COLOR_ZERO2 = "106DED";

        /// <summary>
        /// 红色
        /// </summary>
        public const string COLOR_RED = "A83131";

        /// <summary>
        /// 橙色 
        /// </summary>
        public const string COLOR_ORANGE = "DE4D08";

        /// <summary>
        /// 黄色
        /// </summary>
        public const string COLOR_YELLOW = "D5CB6C";

        /// <summary>
        /// 绿色
        /// </summary>
        public const string COLOR_GREEN = "33B1B0";

        /// <summary>
        /// 蓝色 
        /// </summary>
        public const string COLOR_BLUE = "2762BD";

        /// <summary>
        /// 紫色
        /// </summary>
        public const string COLOR_PURPLE = "865FC5";

        public static string Red(string format, params object[] args)
        {
            return C(COLOR_RED, format, args);
        }

        /// <summary>
        /// 比较重要的日志信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Zero1(string format, params object[] args)
        {
            string color = COLOR_ZERO1_PRO;
#if UNITY_EDITOR            
            if(!UnityEditor.EditorGUIUtility.isProSkin)
            {
                color = COLOR_ZERO1;
            }
#endif
            return C(color, format, args);
        }

        /// <summary>
        /// 不是太重要的日志信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Zero2(string format, params object[] args)
        {
            string color = COLOR_ZERO2_PRO;
#if UNITY_EDITOR            
            if (!UnityEditor.EditorGUIUtility.isProSkin)
            {
                color = COLOR_ZERO2;
            }
#endif
            return C(color, format, args);
        }

        public static string Orange(string format, params object[] args)
        {
            return C(COLOR_ORANGE, format, args);
        }

        public static string Yellow(string format, params object[] args)
        {
            return C(COLOR_YELLOW, format, args);
        }

        public static string Green(string format, params object[] args)
        {
            return C(COLOR_GREEN, format, args);
        }

        public static string Blue(string format, params object[] args)
        {
            return C(COLOR_BLUE, format, args);
        }

        public static string Purple(string format, params object[] args)
        {
            return C(COLOR_PURPLE, format, args);
        }

        /// <summary>
        /// 彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static string C(string color, string format, params object[] args)
        {
            if (null == format)
            {
                return null;
            }

            var message = string.Format("<color=#{0}>{1}</color>", color, string.Format(format,args));
            return message;
        }
    }
}