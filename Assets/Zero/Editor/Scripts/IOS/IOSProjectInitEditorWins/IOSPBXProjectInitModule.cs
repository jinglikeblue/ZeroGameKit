﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor.IOS
{
    class IOSPBXProjectInitModule : BaseXCodeConfigEditorModule
    {
        public enum ETargetGuid
        {
            MAIN,
            FRAMEWORK,
        }

        IOSProjectInitConfigVO.PBXProjectSettingVO _pbxVO;

        public IOSPBXProjectInitModule(EditorWindow editorWin, ETargetGuid targetGuid) : base(editorWin)
        {
            switch (targetGuid)
            {
                case ETargetGuid.MAIN:
                    _pbxVO = Cfg.main;
                    break;
                case ETargetGuid.FRAMEWORK:
                    _pbxVO = Cfg.framework;
                    break;
            }


            frameworkToProjectList = _pbxVO.frameworkToProjectList;
            frameworksToOptionalList = _pbxVO.frameworksToOptionalList;
            file2BuildList = _pbxVO.file2BuildList;
            setBuildProperty = _pbxVO.toSetBuildPropertyList;
            addBuildProperty = _pbxVO.toAddBuildPropertyList;
        }

        void CacheConfig()
        {
            _pbxVO.frameworkToProjectList = frameworkToProjectList;
            _pbxVO.frameworksToOptionalList = frameworksToOptionalList;
            _pbxVO.file2BuildList = file2BuildList;
            _pbxVO.toSetBuildPropertyList = setBuildProperty;
            _pbxVO.toAddBuildPropertyList = addBuildProperty;
        }

        [Title("PBXProject 配置", titleAlignment: TitleAlignments.Centered)]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            CacheConfig();
            SaveConfigFile();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CacheConfig();
        }

        IOSProjectInitConfigVO.FrameworkInfoVO AddFramework()
        {
            return new IOSProjectInitConfigVO.FrameworkInfoVO();
        }

        [Space(20)]
        [InfoBox("把指定的 (系统自带的)framework 文件添加到 [Build Phases] 中的 [Link Binary With Libraries]")]
        [LabelText("AddFrameworkToProject"), ListDrawerSettings(DraggableItems = false, Expanded = true, CustomAddFunction = "AddFramework")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public IOSProjectInitConfigVO.FrameworkInfoVO[] frameworkToProjectList;

        [Space(20)]
        [InfoBox("把第三方framework的Status设置为Optional")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public string[] frameworksToOptionalList;

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

        [Space(20)]
        [ShowInInspector]
        [InfoBox("添加 [Build Settings]中的参数")]
        [LabelText("AddBuildProperty"), DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Value")]
        public Dictionary<string, string> addBuildProperty;
    }
}