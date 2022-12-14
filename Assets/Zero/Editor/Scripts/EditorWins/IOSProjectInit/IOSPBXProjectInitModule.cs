using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    class IOSPBXProjectInitModule : AEditorModule
    {
        public enum ETargetGuid
        {
            MAIN,
            FRAMEWORK,
        }

        public const string CONFIG_NAME = "ios_project_config.json";

        IOSProjectInitConfigVO _cfg;

        IOSProjectInitConfigVO.PBXProjectSettingVO _pbxVO;

        public IOSPBXProjectInitModule(EditorWindow editorWin, ETargetGuid targetGuid) : base(editorWin)
        {
            _cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(CONFIG_NAME);
            if (null == _cfg)
            {
                _cfg = new IOSProjectInitConfigVO();
            }

            switch (targetGuid)
            {
                case ETargetGuid.MAIN:
                    _pbxVO = _cfg.main;
                    break;
                case ETargetGuid.FRAMEWORK:
                    _pbxVO = _cfg.framework;
                    break;
            }


            frameworkToProjectList = _pbxVO.frameworkToProjectList;
            file2BuildList = _pbxVO.file2BuildList;
            setBuildProperty = _pbxVO.buildPropertyList;
        }

        [Title("PBXProject 配置", titleAlignment: TitleAlignments.Centered)]
        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            _pbxVO.frameworkToProjectList = frameworkToProjectList;
            _pbxVO.file2BuildList = file2BuildList;
            _pbxVO.buildPropertyList = setBuildProperty;

            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
            editorWin.ShowTip("保存成功!");
        }

        [Space(20)]
        [InfoBox("把指定的 framework 文件添加到 [Build Phases] 中的 [Link Binary With Libraries]")]
        [LabelText("AddFrameworkToProject"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] frameworkToProjectList;

        [Space(20)]
        [ShowInInspector]
        [InfoBox("把指定的 tbd 文件添加到 [Build Phases] 中的 [Link Binary With Libraries]")]
        [LabelText("AddFileToBuild"), DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Project Path")]
        public Dictionary<string, string> file2BuildList;

        [Space(20)]
        [ShowInInspector]
        [InfoBox("修改 [Build Settings]中的参数")]
        [LabelText("SetBuildProperty"), DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Value")]
        public Dictionary<string, string> setBuildProperty;
    }
}
