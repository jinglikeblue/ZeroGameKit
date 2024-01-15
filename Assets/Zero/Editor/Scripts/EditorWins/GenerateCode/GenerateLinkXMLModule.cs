using Jing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 生成「link.xml」文件模块
    /// </summary>
    public class GenerateLinkXMLModule : AEditorModule
    {
        string CONFIG_NAME = "generate_link_xml_config.json";    
        
        const string NONEXISTENT_FLAG = "[*不存在*]";

        public class ConfigVO
        {
            [HideReferenceObjectPicker]
            [HideLabel]
            public class FolderItemVO
            {
                /// <summary>
                /// 文件夹名称
                /// </summary>
                [FolderPath(RequireExistingPath = true)]
                [LabelText("文件夹路径")]
                public string folderPath;
                /// <summary>
                /// 白名单
                /// </summary>
                [LabelText("关键词白名单")]
                [ListDrawerSettings(Expanded = true, DraggableItems = false)]
                public List<string> whitelist = new List<string>();
                /// <summary>
                /// 黑名单
                /// </summary>
                [LabelText("关键词黑名单")]
                [ListDrawerSettings(Expanded = true, DraggableItems = false)]
                public List<string> blacklist = new List<string>();
            }

            /// <summary>
            /// 要引入的文件夹
            /// </summary>
            public List<string> includeDirs;

            /// <summary>
            /// 要引入的Dll列表
            /// </summary>
            public List<string> includeDlls;

            public List<FolderItemVO> folderItemList;
        }

        ConfigVO _cfg;

        public GenerateLinkXMLModule(EditorWindow editorWin) : base(editorWin)
        {
            _cfg = EditorConfigUtil.LoadConfig<ConfigVO>(CONFIG_NAME);
            if (null == _cfg.includeDirs)
            {
                _cfg.includeDirs = new List<string>();
            }
            if (null == _cfg.includeDlls)
            {
                _cfg.includeDlls = new List<string>();
            }
            if (null == _cfg.folderItemList)
            {
                _cfg.folderItemList = new List<ConfigVO.FolderItemVO>();
            }

            includeDirs = _cfg.includeDirs;
            includeDlls = _cfg.includeDlls;
            folders = _cfg.folderItemList;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            CheckExistsAndRefreshPreviewList();
        }

        [Title("link.xml 生成", TitleAlignment = TitleAlignments.Centered)]
        [InfoBox("IL2CPP在打包时会自动对Unity工程的DLL进行裁剪，将代码中没有引用到的类型裁剪掉，以达到减小发布后ipa包的尺寸的目的。然而在实际使用过程中，很多类型有可能会被意外剪裁掉，造成运行时抛出找不到某个类型的异常。特别是通过反射等方式在编译时无法得知的函数调用，在运行时都很有可能遇到问题。" +
            "Unity提供了一个方式来告诉Unity引擎，哪些类型是不能够被剪裁掉的。具体做法就是在Unity工程的Assets目录中建立一个叫link.xml的XML文件指定你需要保留的类型")]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-9999)]
        void SaveConfig()
        {
            for (int i = 0; i < includeDirs.Count; i++)
            {
                includeDirs[i] = includeDirs[i].Replace(NONEXISTENT_FLAG, "");
            }

            for (int i = 0; i < includeDlls.Count; i++)
            {
                includeDlls[i] = includeDlls[i].Replace(NONEXISTENT_FLAG, "");
            }

            _cfg.folderItemList = folders;

            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);            
        }      

        [PropertySpace(10)]
        [Title("配置相关")]        
        [Button("添加扫描目录", ButtonSizes.Large)]
        [PropertyOrder(-100)]
        void AddDir()
        {
            string dir = EditorUtility.OpenFolderPanel("扫描目录", Application.dataPath, "");
            if (string.IsNullOrEmpty(dir) || includeDirs.Contains(dir))
            {
                return;
            }
            
            var relativePath = FileUtility.GetRelativePath(ZeroEditorConst.PROJECT_PATH, dir);
            if(null != relativePath)
            {
                dir = relativePath;
            }

            includeDirs.Add(dir);
            OnListChange();
        }
        
        [LabelText("扫描的Dll文件夹列表"), ListDrawerSettings(Expanded = true, HideAddButton = true, DraggableItems = false)]
        [DisplayAsString]
        [OnValueChanged("OnListChange", includeChildren: true)]
        [PropertyOrder(-99)]
        public List<string> includeDirs = new List<string>();

        [ListDrawerSettings(AlwaysAddDefaultValue = true, CustomAddFunction = "AddSearchFolder", DraggableItems = false)]
        [LabelText("测试列表")][ShowInInspector]
        public List<ConfigVO.FolderItemVO> folders = new List<ConfigVO.FolderItemVO>();

        void AddSearchFolder()
        {
            folders.Add(new ConfigVO.FolderItemVO());
        }

        [Button("添加Dll文件", ButtonSizes.Large)]
        [PropertyOrder(-98)]
        void AddDll()
        {
            string dll = EditorUtility.OpenFilePanel("Dll文件", Application.dataPath, "dll");
            if (string.IsNullOrEmpty(dll) || includeDlls.Contains(dll))
            {
                return;
            }

            var fi = new FileInfo(dll);

            var relativePath = FileUtility.GetRelativePath(ZeroEditorConst.PROJECT_PATH, fi.DirectoryName);
            if (null != relativePath)
            {
                dll = FileUtility.CombinePaths(relativePath, fi.Name);
            }            

            includeDlls.Add(dll);
            OnListChange();
        }
        
        [LabelText("添加的Dll文件列表"), ListDrawerSettings(Expanded = true, HideAddButton = true, DraggableItems = false)]
        [DisplayAsString]
        [OnValueChanged("OnListChange", includeChildren: true)]
        [PropertyOrder(-97)]
        public List<string> includeDlls = new List<string>();

        void OnListChange()
        {
            Debug.Log("列表数据变化");            
            SaveConfig();
            CheckExistsAndRefreshPreviewList();
        }

        [LabelText("是否保留Editor DLL文件"), OnValueChanged("CheckExistsAndRefreshPreviewList")]
        [PropertyOrder(-96)]
        public bool isKeepEditorAssembly = false;


        [Title("导出相关")]
        [PropertySpace(10)]
        [LabelText("导出位置"), PropertyOrder(-80), DisplayAsString]
        public string path = "assets/link.xml";

        [Button("导出 [link.xml]", ButtonSizes.Large)]
        void CreateLinkXML()
        {
            const string OUTPUT_FILE = "Assets/link.xml";

            new GenerateLinkXMLCommand(outputPreviewList, OUTPUT_FILE).Excute();

            AssetDatabase.Refresh();
            ShowTip("[{0}] 导出完毕!", OUTPUT_FILE);
        }

        [Button("刷新列表", ButtonSizes.Large)]
        void CheckExistsAndRefreshPreviewList()
        {
            for (int i = 0; i < includeDirs.Count; i++)
            {
                if (false == Directory.Exists(includeDirs[i]))
                {
                    includeDirs[i] = $"{NONEXISTENT_FLAG}{includeDirs[i]}";
                }
            }

            for (int i = 0; i < includeDlls.Count; i++)
            {
                if (false == File.Exists(includeDlls[i]))
                {
                    includeDlls[i] = $"{NONEXISTENT_FLAG}{includeDlls[i]}";
                }
            }

            var defaultDir = FileUtility.CombineDirs(true, EditorApplication.applicationContentsPath);
            if (false == includeDirs.Contains(defaultDir))
            {
                includeDirs.Add(defaultDir);
                Debug.LogFormat("[{0}]是必须的", defaultDir);
            }

            RefreshPreviewList();
        }


        
        [LabelText("保留类型预览"), DisplayAsString, PropertyOrder(999), ListDrawerSettings(DraggableItems = false, HideRemoveButton = true, HideAddButton = true, NumberOfItemsPerPage = 20)]
        public List<string> outputPreviewList;

        /// <summary>
        /// 刷新导出预览列表
        /// </summary>
        void RefreshPreviewList()
        {
            HashSet<string> assemblySet = new HashSet<string>();

            foreach (var dll in includeDlls)
            {
                if (File.Exists(dll))
                {
                    assemblySet.Add(Path.GetFileNameWithoutExtension(dll));
                }
            }

            foreach (var dir in includeDirs)
            {
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
                    for (int i = 0; i < files.Length; i++)
                    {
                        var file = files[i];
                        assemblySet.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
            }

            if (null == outputPreviewList)
            {
                outputPreviewList = new List<string>();
            }
            else
            {
                outputPreviewList.Clear();
            }

            if (false == isKeepEditorAssembly)
            {
                HashSet<string> filterSet = new HashSet<string>();
                foreach (var set in assemblySet)
                {
                    var lowerSet = set.ToLower();
                    if (lowerSet.Contains("editor"))
                    {
                        Debug.LogWarning($"[Link.xml] 排除的Assembly: {set}");                        
                    }
                    else
                    {
                        filterSet.Add(set);
                    }
                }
                assemblySet = filterSet;
            }
            outputPreviewList.AddRange(assemblySet);
        }
    }
}
