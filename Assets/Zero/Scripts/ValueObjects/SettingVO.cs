using System.Collections.Generic;

namespace Zero
{
    public class SettingVO
    {
        /// <summary>
        /// 客户端版本号数据
        /// </summary>
        public struct Client
        {
            /// <summary>
            /// 客户端版本
            /// </summary>
            public string version;

            /// <summary>
            /// 客户端地址
            /// </summary>
            public string url;
        }

        public Client client;

        /// <summary>
        /// 网络资源重定向目录。为空表示使用LauncherSetting中配置的netResRoot
        /// </summary>
        public string netResRoot;

        /// <summary>
        /// 启动资源组列表
        /// </summary>
        public string[] startupResGroups;

        /// <summary>
        /// 配置的参数
        /// </summary>
        public Dictionary<string, string> startupParams;

        /// <summary>
        /// LauncherSetting配置：是否允许打印日志
        /// </summary>
        public LauncherSettingParam<bool> lsLogEnable = new LauncherSettingParam<bool>();

        /// <summary>
        /// LauncherSetting配置：使用dll
        /// </summary>
        public LauncherSettingParam<bool> lsUseDll = new LauncherSettingParam<bool>();

        public class LauncherSettingParam<T> 
        {
            public bool isOverride = false;
            public T value;
        }
    }
}