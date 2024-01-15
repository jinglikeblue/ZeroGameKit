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
    class IOSCapabilitySettingModule : AEditorModule
    {
        public const string CONFIG_NAME = "ios_project_config.json";

        IOSProjectInitConfigVO _cfg;

        public IOSCapabilitySettingModule(EditorWindow editorWin) : base(editorWin)
        {
            _cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(CONFIG_NAME);
            if (null == _cfg)
            {
                _cfg = new IOSProjectInitConfigVO();
            }
            entitlementFilePath = _cfg.capabilitySetting.entitlementFilePath;
            inAppPurchase = _cfg.capabilitySetting.inAppPurchase;
            pushNotifications = _cfg.capabilitySetting.pushNotifications.enable;
            isPushNotificationsDevelopment = _cfg.capabilitySetting.pushNotifications.development;
#if UNITY_IPHONE
            backgroundModes = _cfg.capabilitySetting.backgroundModes;
#endif
            associatedDomains = _cfg.capabilitySetting.associatedDomains;
            signInWithApple = _cfg.capabilitySetting.signInWithApple;
            accessWiFiInformation = _cfg.capabilitySetting.accessWiFiInformation;
            gameCener = _cfg.capabilitySetting.gameCener;
        }

        [Title("Capability 配置", titleAlignment: TitleAlignments.Centered)]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            _cfg.capabilitySetting.entitlementFilePath = entitlementFilePath;
            _cfg.capabilitySetting.inAppPurchase = inAppPurchase;
            _cfg.capabilitySetting.pushNotifications.enable = pushNotifications;
            _cfg.capabilitySetting.pushNotifications.development = isPushNotificationsDevelopment;
#if UNITY_IPHONE
            _cfg.capabilitySetting.backgroundModes = backgroundModes;
#endif
            _cfg.capabilitySetting.associatedDomains = associatedDomains;
            _cfg.capabilitySetting.signInWithApple = signInWithApple;
            _cfg.capabilitySetting.accessWiFiInformation = accessWiFiInformation;
            _cfg.capabilitySetting.gameCener = gameCener;

            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
            editorWin.ShowTip("保存成功!");
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
