using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor.IOS
{
    class IOSProjectInitEditorWin : OdinMenuEditorWindow
    {
        public static void Open()
        {
            var win = GetWindow<IOSProjectInitEditorWin>("iOS构建自动化配置", true);
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 600);
            win.position = rect;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();
            tree.Config.DrawSearchToolbar = false;
            tree.Add("全局配置", new IOSGlobalSettingModule(this));
            tree.Add("Capability 配置", new IOSCapabilitySettingModule(this));
            tree.Add("Info.plist 配置", new IOSInfoplistInitModule(this));
            tree.Add("CopyFiles 配置", new IOSCopyFilesToXCodeModule(this));
            tree.Add("Unity-iPhone 配置", new IOSPBXProjectInitModule(this, IOSPBXProjectInitModule.ETargetGuid.MAIN));
            tree.Add("UnityFramework 配置", new IOSPBXProjectInitModule(this, IOSPBXProjectInitModule.ETargetGuid.FRAMEWORK));            
            return tree;
        }
    }
}
