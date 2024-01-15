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
    class IOSGlobalSettingModule:AEditorModule
    {
        public const string CONFIG_NAME = "ios_project_config.json";

        IOSProjectInitConfigVO _cfg;

        public IOSGlobalSettingModule(EditorWindow editorWin) : base(editorWin)
        {
            _cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(CONFIG_NAME);
            if (null == _cfg)
            {
                _cfg = new IOSProjectInitConfigVO();
            }

            isEnable = _cfg.isEnable;
            appIconSetList = _cfg.appIconSetList;
        }

        [Title("全局配置", titleAlignment: TitleAlignments.Centered)]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            _cfg.isEnable = isEnable;
            _cfg.appIconSetList = appIconSetList;

            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
            editorWin.ShowTip("保存成功!");
        }

        [Title("功能开关")]
        [Space(20)]
        [InfoBox("「启用」的情况下，发布「iOS」的「xcode」项目后，会按照本工具的配置对项目进行自动配置")]
        [LabelText("是否激活自动化配置功能")]
        public bool isEnable;

        [Title("App Icon 配置")]
        [Space(20)]
        [InfoBox("如果项目需要有多套应用图标替换，那么可以在这里进行其它ICON的配置。文件夹名字通常为xxx.appiconset。如果配置数据存在，则会自动将工程Build Settings中 Include all app icon assets改为 YES！")]
        [LabelText("APP ICON图集文件夹列表")][FolderPath]
        public string[] appIconSetList = new string[0];
    }
}
