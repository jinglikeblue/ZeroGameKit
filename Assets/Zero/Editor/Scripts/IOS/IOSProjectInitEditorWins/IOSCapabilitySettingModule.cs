using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor.IOS
{
    class IOSCapabilitySettingModule : BaseXCodeConfigEditorModule
    {
        public IOSCapabilitySettingModule(EditorWindow editorWin) : base(editorWin)
        {
            entitlementFilePath = Cfg.capabilitySetting.entitlementFilePath;
            inAppPurchase = Cfg.capabilitySetting.inAppPurchase;
            pushNotifications = Cfg.capabilitySetting.pushNotifications.enable;
            isPushNotificationsDevelopment = Cfg.capabilitySetting.pushNotifications.development;
#if UNITY_IPHONE
            backgroundModes = Cfg.capabilitySetting.backgroundModes;
#endif
            associatedDomains = Cfg.capabilitySetting.associatedDomains;
            signInWithApple = Cfg.capabilitySetting.signInWithApple;
            accessWiFiInformation = Cfg.capabilitySetting.accessWiFiInformation;
            gameCener = Cfg.capabilitySetting.gameCener;
        }

        [Title("Capability 配置", titleAlignment: TitleAlignments.Centered)]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            Cfg.capabilitySetting.entitlementFilePath = entitlementFilePath;
            Cfg.capabilitySetting.inAppPurchase = inAppPurchase;
            Cfg.capabilitySetting.pushNotifications.enable = pushNotifications;
            Cfg.capabilitySetting.pushNotifications.development = isPushNotificationsDevelopment;
#if UNITY_IPHONE
            Cfg.capabilitySetting.backgroundModes = backgroundModes;
#endif
            Cfg.capabilitySetting.associatedDomains = associatedDomains;
            Cfg.capabilitySetting.signInWithApple = signInWithApple;
            Cfg.capabilitySetting.accessWiFiInformation = accessWiFiInformation;
            Cfg.capabilitySetting.gameCener = gameCener;

            SaveConfigFile();
        }

        [Space(20)]
        [Title("权限描述文件")]
        [InfoBox("如果不需要创建权限描述文件，则填写空字符串即可。权限描述文件的填写必须以后缀名.entitlements结尾")]
        [LabelText("entitlements file")]
        public string entitlementFilePath;

        [Space(20)]
        [DisplayAsString]
        [Title("Apple支付")]
        [LabelText("In-App Purchase")]
        public bool inAppPurchase = false;

        [Space(20)]
        [Title("通知推送")]
        [LabelText("Push Notifications")]
        public bool pushNotifications = false;

        [LabelText("Development")]
        [ShowIf("pushNotifications")]
        public bool isPushNotificationsDevelopment = false;

#if UNITY_IPHONE

        [Space(20)]
        [Title("后台运行")]
        [LabelText("Background Modes")]
        public UnityEditor.iOS.Xcode.BackgroundModesOptions backgroundModes = UnityEditor.iOS.Xcode.BackgroundModesOptions.None;

#endif

        [Space(20)]
        [Title("关联域名")]
        [LabelText("Associated Domains")]
        [ListDrawerSettings(DraggableItems = false, Expanded = true, NumberOfItemsPerPage = 5)]
        public string[] associatedDomains = new string[0];

        [Space(20)]
        [Title("Apple登录")]
        [LabelText("Sign In with Apple")]
        public bool signInWithApple = false;

        [Space(20)]
        [Title("获取WiFi信息")]
        [LabelText("Access WiFi Information")]
        public bool accessWiFiInformation = false;

        [Space(20)]
        [Title("苹果游戏中心")]
        [LabelText("Game Center")]
        public bool gameCener = false;
    }
}