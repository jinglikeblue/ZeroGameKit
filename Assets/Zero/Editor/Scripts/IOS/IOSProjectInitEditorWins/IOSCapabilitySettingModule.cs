using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
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
            backgroundModes = _cfg.capabilitySetting.backgroundModes;
            associatedDomains = _cfg.capabilitySetting.associatedDomains;
            signInWithApple = _cfg.capabilitySetting.signInWithApple;            
        }

        [Title("Capability 配置", titleAlignment: TitleAlignments.Centered)]
        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            _cfg.capabilitySetting.entitlementFilePath = entitlementFilePath;
            _cfg.capabilitySetting.inAppPurchase = inAppPurchase;
            _cfg.capabilitySetting.pushNotifications.enable = pushNotifications;
            _cfg.capabilitySetting.pushNotifications.development = isPushNotificationsDevelopment;
            _cfg.capabilitySetting.backgroundModes = backgroundModes;
            _cfg.capabilitySetting.associatedDomains = associatedDomains;
            _cfg.capabilitySetting.signInWithApple = signInWithApple;

            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
            editorWin.ShowTip("保存成功!");
        }

        [Space(20)]
        [LabelText("entitlement 文件")]
        public string entitlementFilePath;

        [Space(20)]        
        [LabelText("Apple Pay功能")]
        public bool inAppPurchase = false;

        [Space(20)]        
        [LabelText("推送功能")]
        public bool pushNotifications = false;

        [LabelText("推送功能,是否开发模式")]
        [ShowIf("pushNotifications")]
        public bool isPushNotificationsDevelopment = false;

        [Space(20)]
        [LabelText("后台运行模式")]
        public BackgroundModesOptions backgroundModes = BackgroundModesOptions.None;

        [Space(20)]
        [LabelText("关联域名")]
        public string[] associatedDomains = new string[0];

        [Space(20)]
        [LabelText("Apple Id 登录")]
        public bool signInWithApple = false;
    }
}
