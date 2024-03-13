using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 应用工具类
    /// </summary>
    public static class ApplicationUtility
    {
        /// <summary>
        /// 是否是Editor环境
        /// </summary>
        public static bool IsEditor
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.LinuxEditor:
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.OSXEditor:
                        return true;
                }

                return false;
            }
        }
    }
}