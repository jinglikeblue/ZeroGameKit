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
namespace ZeroEditor
{
    /// <summary>
    /// SpriteAtlasTools工具
    /// </summary>
    class SpriteAtlasToolsEditorWin : OdinEditorWindow
    {       
        public static SpriteAtlasToolsEditorWin Open()
        {
            var win = GetWindow<SpriteAtlasToolsEditorWin>("SpriteAtlas Tools");
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(860, 700);
            return win;
        }

        SpriteAtlasToolsConfigVO _cfg;

        protected override void OnEnable()
        {
            base.OnEnable();
            SpriteAtlasToolsUtility.onAddSpriteAtlas += OnUtilityAddSpriteAtlas;            

            LoadConfigFile();
        }

        private void OnDisable()
        {
            SpriteAtlasToolsUtility.onAddSpriteAtlas -= OnUtilityAddSpriteAtlas;            
        }

        private void OnUtilityAddSpriteAtlas()
        {
            LoadConfigFile();
        }

        void LoadConfigFile()
        {
            _cfg = EditorConfigUtil.LoadConfig<SpriteAtlasToolsConfigVO>(SpriteAtlasToolsUtility.CONFIG_NAME);

            spriteAtlasSaveDirPath = _cfg.spriteAtlasSaveDirPath;
            packingTextureWidthLimit = _cfg.packingTextureWidthLimit;
            packingTextureHeightLimit = _cfg.packingTextureHeightLimit;
            itemList.Clear();
            for (var i = 0; i < _cfg.itemList.Count; i++)
            {
                itemList.Add(new SpriteAtlasItemEditor(this, _cfg.itemList[i]));
            }
        }

        [Button("保存配置", ButtonSizes.Large), PropertyOrder(0)]
        void SaveConfig()
        {
            _cfg.spriteAtlasSaveDirPath = spriteAtlasSaveDirPath;
            _cfg.packingTextureWidthLimit = packingTextureWidthLimit;
            _cfg.packingTextureHeightLimit = packingTextureHeightLimit;
            _cfg.itemList.Clear();
            for (var i = 0; i < itemList.Count; i++)
            {
                _cfg.itemList.Add(itemList[i].ToSpriteAtlasItemVO());
            }
            EditorConfigUtil.SaveConfig(_cfg, SpriteAtlasToolsUtility.CONFIG_NAME);
        }



        [Title("Editor配置")]
        [InfoBox("工具创建的SpriteAtlas文件都会保存在该目录下")]
        [LabelText("SpriteAtlas文件保存目录"), LabelWidth(160)]
        [FolderPath(AbsolutePath = false, ParentFolder = "./", UseBackslashes = false)]
        [PropertyOrder(10)]
        [InlineButton("SelectSpriteAtlasDir", "查看")]
        public string spriteAtlasSaveDirPath;

        void SelectSpriteAtlasDir()
        {
            var isSuccess = ZeroEditorUtility.SetPathToSelection(spriteAtlasSaveDirPath);
            if (!isSuccess)
            {
                this.ShowTip("路径不存在：构建时将自动创建");                
            }           
        }

        [Title("打包到纹理集的资源大小限制", "大于等于配置的Width或Height的图片，会在构建时自动排除出SpriteAtlas")]        
        [LabelText("宽度(Width)"),LabelWidth(80)]        
        [PropertyOrder(11)]        
        public int packingTextureWidthLimit;

        [LabelText("高度(Height)")]
        [PropertyOrder(11), LabelWidth(80)]        
        public int packingTextureHeightLimit;        

        [Title("SpriteAtlas配置")]        
        [InfoBox("只需要配置好纹理放置的目录以及生成方式即可，SpriteAtlas的文件名会自动生成")]        
        [LabelText("SpriteAtlas文件数据")]
        [PropertyOrder(20)]
        [HideReferenceObjectPicker]
        //[TableList(AlwaysExpanded = true, NumberOfItemsPerPage = 5, DrawScrollView = false, ShowPaging = true)]
        [ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 10, AlwaysAddDefaultValue = true, CustomAddFunction = "AddSpriteAtlasItem", DraggableItems = false)]
        public List<SpriteAtlasItemEditor> itemList = new List<SpriteAtlasItemEditor>();

        public void AddSpriteAtlasItem()
        {
            itemList.Add(new SpriteAtlasItemEditor(this, null));
        }

        [Title("构建")]
        [InfoBox("构建将完成以下操作：\r\n 根据「SpriteAtlas文件数据」创建或更新「SpriteAtlas保存目录」中的SpriteAtlas文件")]
        [Button("构建所有的SpriteAtlas文件", ButtonSizes.Large)]
        [PropertyOrder(30)]
        void BuildAll()
        {
            if (itemList.Count == 0)
            {
                return;
            }

            EditorUtility.DisplayProgressBar("进度", "SpriteAtlas文件构建中...", 0);

            //检查保存目录是否存在，不在则生成
            if (!Directory.Exists(spriteAtlasSaveDirPath))
            {
                Directory.CreateDirectory(spriteAtlasSaveDirPath);
            }

            for (var i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                var progress = (i + 1f) / itemList.Count;
                EditorUtility.DisplayProgressBar("进度", "SpriteAtlas文件构建中...", progress);

                SpriteAtlasToolsUtility.BuildSpriteAtlas(spriteAtlasSaveDirPath, item.ToSpriteAtlasItemVO(), packingTextureWidthLimit, packingTextureHeightLimit);
            }

            EditorUtility.ClearProgressBar();
        }
        
        [Button("触发已构建SpriteAtlas文件的[Pack Preview]，刷新纹理集预览", ButtonSizes.Large)]
        [PropertyOrder(31)]
        void PackPreviewAll()
        {
            UnityEditor.U2D.SpriteAtlasUtility.PackAllAtlases(EditorUserBuildSettings.activeBuildTarget);    
            // SpriteAtlasToolsUtility.PackPreview(spriteAtlasSaveDirPath);
        }

        [Button("清理Editor下的SpriteAtlas缓存", ButtonSizes.Large)]
        [PropertyOrder(32)]
        void ClearLibraryCache()
        {
            SpriteAtlasToolsUtility.ClearLibraryCache();
        }

        public struct SpriteAtlasItemEditor
        {
            SpriteAtlasToolsEditorWin _win;

            /// <summary>
            /// 是否子目录单独创建SpriteAtlas
            /// </summary>
            //[TableColumnWidth(190, false)]
            [HorizontalGroup("Item", MaxWidth = 190)]
            [LabelText("子目录单独生成SpriteAtlas"), LabelWidth(150)]
            //[ToggleLeft]            
            public bool isSubDirSplit;

            /// <summary>
            /// 打包纹理集的目录
            /// </summary>
            [LabelText("纹理资源目录"), LabelWidth(100)]
            [HideLabel]
            [FolderPath(AbsolutePath = false, ParentFolder = "./", UseBackslashes = false, RequireExistingPath = true)]
            [InlineButton("SelectFile", "配置SpriteAtlas")]
            [HorizontalGroup("Item")]
            public string texturesDirPath;

            void SelectFile()
            {                
                var name = SpriteAtlasToolsUtility.GenerateSpriteAtlasNameByPath(texturesDirPath);
                var filePath = FileUtility.CombinePaths(_win.spriteAtlasSaveDirPath, name);
                var isSuccess = ZeroEditorUtility.SetPathToSelection(filePath);
                if (isSuccess)
                {
                    return;                    
                }

                isSuccess = ZeroEditorUtility.SetPathToSelection(_win.spriteAtlasSaveDirPath);

                //尝试选中目录
                if (false == isSuccess)
                {
                    _win.ShowTip($"[{name}]不存在：未构建");
                }
            }

            //[TableColumnWidth(60, false)]
            [Button("构建")]            
            [HorizontalGroup("Item", MaxWidth = 60)]
            void Build()
            {
                EditorUtility.DisplayProgressBar("进度", "SpriteAtlas文件构建中...", 0);
                SpriteAtlasToolsUtility.BuildSpriteAtlas(_win.spriteAtlasSaveDirPath, ToSpriteAtlasItemVO(), _win.packingTextureWidthLimit, _win.packingTextureHeightLimit);
                EditorUtility.ClearProgressBar();
            }

            public SpriteAtlasItemEditor(SpriteAtlasToolsEditorWin win, SpriteAtlasToolsConfigVO.SpriteAtlasItemVO vo)
            {
                if (null == vo)
                {
                    vo = new SpriteAtlasToolsConfigVO.SpriteAtlasItemVO();
                }

                this.texturesDirPath = vo.texturesDirPath;
                this.isSubDirSplit = vo.isSubDirSplit;
                _win = win;
            }

            public SpriteAtlasToolsConfigVO.SpriteAtlasItemVO ToSpriteAtlasItemVO()
            {
                var vo = new SpriteAtlasToolsConfigVO.SpriteAtlasItemVO();
                vo.texturesDirPath = texturesDirPath;
                vo.isSubDirSplit = isSubDirSplit;
                return vo;
            }
        }
    }
}
