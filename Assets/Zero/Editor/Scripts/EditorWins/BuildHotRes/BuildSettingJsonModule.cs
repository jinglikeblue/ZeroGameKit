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
using Zero;

namespace ZeroEditor
{
    public class BuildSettingJsonModule : AEditorModule
    {
        const string CONFIG_NAME = "setting_config.json";

        /// <summary>
        /// 设置数据
        /// </summary>
        SettingVO cfg;

        public BuildSettingJsonModule(EditorWindow editorWin) : base(editorWin)
        {
            SetCfg(EditorConfigUtil.LoadConfig<SettingVO>(CONFIG_NAME));
        }

        void SetCfg(SettingVO cfg)
        {
            this.cfg = cfg;
            version = cfg.client.version;
            url = cfg.client.url;
            netResRoot = cfg.netResRoot;
            startupResGroups = cfg.startupResGroups;
            startupParams = cfg.startupParams;            
            if (null == startupParams)
            {
                startupParams = new Dictionary<string, string>();
            }
        }

        void UpdateCfg()
        {
            cfg.client.version = version;
            cfg.client.url = url;
            cfg.netResRoot = netResRoot;
            cfg.startupResGroups = startupResGroups;
            cfg.startupParams = startupParams;            
        }

        [Title("setting.json 热更配置文件", TitleAlignment = TitleAlignments.Centered)]
        [Button("保存配置", buttonSize: ButtonSizes.Medium), PropertyOrder(-1)]
        void SaveConfig()
        {
            UpdateCfg();
            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);
        }

        [Button("复制本地「setting.json」参数", buttonSize: ButtonSizes.Medium), PropertyOrder(-1)]
        void LoadExistSettingJson()
        {
            var selectedFile = EditorUtility.OpenFilePanel("选择文件", Application.dataPath, "json");
            if (false == string.IsNullOrEmpty(selectedFile))
            {
                try
                {
                    var jsonStr = File.ReadAllText(selectedFile);
                    SetCfg(Json.ToObject<SettingVO>(jsonStr));
                }
                catch (Exception e)
                {
                    Debug.LogError("读取选择的setting.json文件失败：" + selectedFile);
                    Debug.LogError(e);
                }
            }
        }

        [Title("客户端版本")]
        [InfoBox("当客户端版本号低于setting文件版本号时，将打开更新页面")]
        [LabelText("版本号")]
        public string version;

        [LabelText("更新地址URL")]
        public string url;

        [Title("远端资源重定向")]
        [LabelText("重定向资源根目录URL：")]
        [InfoBox("如果该setting.json对应的热更资源不在同级别目录下（在其它url地址）。那么可以在这里重定向为对应的网络资源根目录。否则不用刻意填写")]
        public string netResRoot;

        [Space(10)]
        [InfoBox("客户端启动运行所必需下载的网络资源组，通过指定资源路径(文件或目录，比如[ab/h.ab]或[dll])来批量下载。如果要下载所有资源，则指定为[/]即可\n")]
        [LabelText("启动资源组"), ListDrawerSettings(NumberOfItemsPerPage = 7, Expanded = false)]
        public string[] startupResGroups;

        [InfoBox("使用[Runtime.GetStartupParams]方法，在运行时获取")]
        [Title("启动参数配置"),ShowInInspector]
        public Dictionary<string, string> startupParams;


        [Button("发布「setting.json」", buttonSize: ButtonSizes.Medium), PropertyOrder(998)]
        void BuildSettingJsonFile()
        {
            if (false == Directory.Exists(ZeroEditorConst.PUBLISH_RES_ROOT_DIR))
            {
                Directory.CreateDirectory(ZeroEditorConst.PUBLISH_RES_ROOT_DIR);
            }

            var filePath = FileUtility.CombinePaths(ZeroEditorConst.PUBLISH_RES_ROOT_DIR, ZeroConst.SETTING_FILE_NAME);
            if (File.Exists(filePath) && false == EditorUtility.DisplayDialog("警告！", "已存在文件「setting.json」，是否覆盖？", "Yes", "No"))
            {
                return;
            }

            UpdateCfg();
            string jsonStr = Json.ToJsonIndented(cfg);
            File.WriteAllText(filePath, jsonStr);

            if (isOpenPublishDir)
            {
                OpenPublishDir();
            }
            else
            {
                EditorUtility.DisplayDialog("", "发布完成!", "确定");
            }
        }

        [LabelText("发布完成后打开发布目录"), ToggleLeft, PropertyOrder(999)]
        [InlineButton("OpenPublishDir", "打开发布目录")]
        public bool isOpenPublishDir = true;

        void OpenPublishDir()
        {
            //打开目录
            ZeroEditorUtility.OpenDirectory(ZeroEditorConst.PUBLISH_RES_ROOT_DIR);
        }
    }
}
