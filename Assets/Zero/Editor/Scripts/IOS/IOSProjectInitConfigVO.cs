using System.Collections.Generic;

namespace ZeroEditor.IOS
{
    /// <summary>
    /// iOS项目初始化配置
    /// </summary>
    public class IOSProjectInitConfigVO
    {
        /// <summary>
        /// 是否激活自动化配置功能
        /// </summary>
        public bool isEnable = true;

        /// <summary>
        /// APP图标集合列表
        /// </summary>
        public string[] appIconSetList = new string[0];
        /// 要拷贝的文件
        /// </summary>
        public CopyInfoVO[] copyInfoList = new CopyInfoVO[0];

        /// <summary>
        /// 功能权限
        /// </summary>
        public CapabilitySettingVO capabilitySetting = new CapabilitySettingVO();


        #region PBXProject

        /// <summary>
        /// 对应pbx.GetUnityMainTargetGuid()
        /// </summary>
        public PBXProjectSettingVO main = new PBXProjectSettingVO();

        /// <summary>
        /// 对应pbx.GetUnityFrameworkTargetGuid()
        /// 从Unity2019LTS开始，XCode项目分为主工程和库项目两部分
        /// </summary>
        public PBXProjectSettingVO framework = new PBXProjectSettingVO();

        #endregion

        #region info.plist

        /// <summary>
        /// Info.plist高级参数配置
        /// </summary>
        public List<IOSInfoPListDictionaryItem> pListAdvancedDataList = new List<IOSInfoPListDictionaryItem>();

        /// <summary>
        /// Info.plist参数配置
        /// </summary>
        public Dictionary<string, string> pListDataList = new Dictionary<string, string>();

        /// <summary>
        /// 要删除的配置列表
        /// </summary>
        public string[] deletePListDataKeyList = new string[0];

        /// <summary>
        /// urlscheme配置
        /// </summary>
        public string[] urlSchemeList = new string[0];

        /// <summary>
        /// 信任urlscheme配置
        /// </summary>
        public string[] appQueriesSchemeList = new string[0];

        #endregion


        public class PBXProjectSettingVO
        {
            /// <summary>
            /// framework库配置
            /// </summary>
            public FrameworkInfoVO[] frameworkToProjectList = new FrameworkInfoVO[0];

            /// <summary>
            /// 需要吧Status设置为Optional的第三方framework
            /// </summary>
            public string[] frameworksToOptionalList = new string[0];

            /// <summary>
            /// lib库配置
            /// </summary>
            public Dictionary<string, string> file2BuildList = new Dictionary<string, string>();

            /// <summary>
            /// build参数配置(设置的)
            /// </summary>
            public Dictionary<string, string> toSetBuildPropertyList = new Dictionary<string, string>();

            /// <summary>
            /// build参数配置(添加的)
            /// </summary>
            public Dictionary<string, string> toAddBuildPropertyList = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加的framework库的信息
        /// </summary>
        public class FrameworkInfoVO
        {            
            /// <summary>            
            /// 库名称，扩展名必须是".framework"
            /// </summary>
            public string name;

            /// <summary>
            /// true 表示 optinal; 
            /// false 表示 required
            /// </summary>
            public bool isWeak = true;
        }

        public class CopyInfoVO
        {
            /// <summary>
            /// 相对于Unity工程目录的路径
            /// </summary>
            public string fromPath;

            /// <summary>
            /// 相对于XCode工程目录的路径
            /// </summary>
            public string toPath;

            /// <summary>
            /// 是否添加到Main的编译中
            /// </summary>
            public bool isAddToMain = false;

            /// <summary>
            /// 是否添加到Framework的编译中
            /// </summary>
            public bool isAddToFramework = false;

            /// <summary>
            /// 是否将文件或目录添加到【Build Phases】的【Copy Bundle Resources】中
            /// </summary>
            public bool isAddPathToResourcesBuildPhase = false;
        }

        /// <summary>
        /// 导出包功能需求
        /// </summary>
        public class CapabilitySettingVO
        {
            public class PushNotificationsVO
            {
                public bool enable = false;
                public bool development = false;
            }

            /// <summary>
            /// entitlement文件路径
            /// </summary>
            public string entitlementFilePath = "Unity-iPhone.entitlements";

            /// <summary>
            /// 苹果支付
            /// </summary>
            public bool inAppPurchase = false;            

            /// <summary>
            /// 推送
            /// </summary>
            public PushNotificationsVO pushNotifications = new PushNotificationsVO();

#if UNITY_IPHONE
            /// <summary>
            /// 后台推送
            /// </summary>
            public UnityEditor.iOS.Xcode.BackgroundModesOptions backgroundModes = UnityEditor.iOS.Xcode.BackgroundModesOptions.None;
#endif

            /// <summary>
            /// 关联域名
            /// </summary>
            public string[] associatedDomains = new string[0];

            /// <summary>
            /// AppleId登录
            /// </summary>
            public bool signInWithApple = false;

            /// <summary>
            /// Access WiFi Information
            /// </summary>
            public bool accessWiFiInformation = false;

            /// <summary>
            /// Game Center
            /// </summary>
            public bool gameCener = false;
        }

    }
}
