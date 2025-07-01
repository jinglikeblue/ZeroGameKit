using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ZeroEditor.IOS;

namespace ZeroEditor.IOS
{
    class IOSInfoplistInitModule : BaseXCodeConfigEditorModule
    {
        public IOSInfoplistInitModule(EditorWindow editorWin) : base(editorWin)
        {
            addPListInfo = Cfg.pListDataList;
            urlSchemeList = Cfg.urlSchemeList;
            deletePListDataKeyList = Cfg.deletePListDataKeyList;
            appQueriesSchemeList = Cfg.appQueriesSchemeList;
            pListAdvancedDataList = Cfg.pListAdvancedDataList;
        }

        [Title("Info.plist 配置", titleAlignment: TitleAlignments.Centered)]
        [Button("保存配置", ButtonSizes.Large), PropertyOrder(-1)]
        void SaveConfig()
        {
            Cfg.pListDataList = addPListInfo;
            Cfg.urlSchemeList = urlSchemeList;
            Cfg.appQueriesSchemeList = appQueriesSchemeList;
            Cfg.deletePListDataKeyList = deletePListDataKeyList;
            Cfg.pListAdvancedDataList = pListAdvancedDataList;
            
            SaveConfigFile();
        }

        void AddIOSInfoPListDictionaryItem()
        {
            pListAdvancedDataList.Add(new IOSInfoPListDictionaryItem());
        }

        [Space(20)]
        [ShowInInspector]
        [InfoBox("将键值添加到 [Info] 的 [Custom iOS Target Properties] 中")]
        [DictionaryDrawerSettings(KeyLabel = "Key", ValueLabel = "Value")]
        [LabelText("添加 info.plist 的参数")]
        public Dictionary<string, string> addPListInfo;

        [Space(20)]        
        [InfoBox("将键值从 [Info] 的 [Custom iOS Target Properties] 中移除")]        
        [LabelText("从 info.plist 移除的参数")]
        public string[] deletePListDataKeyList;

        [Space(20)]
        [InfoBox("编辑 [Info] 的 [URL Types] 中的数据")]
        [LabelText("UrlScheme"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] urlSchemeList;

        [Space(20)]
        [InfoBox("编辑 [Info] 的 [Custom iOS Target Properties] 中 [LSApplicationQueriesScheme] 的数据")]
        [LabelText("LSApplicationQueriesScheme(UrlScheme白名单)"), ListDrawerSettings(DraggableItems = false, NumberOfItemsPerPage = 5, Expanded = true)]
        public string[] appQueriesSchemeList;

        [Space(20)]
        [ShowInInspector]
        [InfoBox("将键值添加到 [Info] 的 [Custom iOS Target Properties] 中。 当参数是array或者dictionary的情况时，在这里进行配置。")]
        [LabelText("info.plist 复杂参数设置")]
        [ListDrawerSettings(CustomAddFunction = "AddIOSInfoPListDictionaryItem", DraggableItems = false)]        
        public List<IOSInfoPListDictionaryItem> pListAdvancedDataList = new List<IOSInfoPListDictionaryItem>();


    }
}
