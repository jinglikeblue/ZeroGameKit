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
        const string CONFIG_NAME = "sprite_atlas_tools_config.json";

        public static SpriteAtlasToolsEditorWin Open()
        {
            return GetWindow<SpriteAtlasToolsEditorWin>("SpriteAtlas Tools");
        }

        SpriteAtlasToolsConfigVO _cfg;

        protected override void OnEnable()
        {
            base.OnEnable();

            _cfg = EditorConfigUtil.LoadConfig<SpriteAtlasToolsConfigVO>(CONFIG_NAME);

            spriteAtlasSaveDirPath = _cfg.spriteAtlasSaveDirPath;
            for (var i = 0; i < _cfg.itemList.Count; i++)
            {
                itemList.Add(new SpriteAtlasItemEditor(_cfg.itemList[i]));
            }
        }

        [LabelText("保存配置"), Button(size: ButtonSizes.Large), PropertyOrder(0)]
        void SaveConfig()
        {
            _cfg.spriteAtlasSaveDirPath = spriteAtlasSaveDirPath;
            _cfg.itemList.Clear();
            for (var i = 0; i < itemList.Count; i++)
            {
                _cfg.itemList.Add(itemList[i].ToSpriteAtlasItemVO());
            }
            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
        }

        [Title("Editor配置")]
        [InfoBox("工具创建的spriteatlas文件都会保存在该目录下")]
        [LabelText("spriteatlas文件保存目录")]
        [FolderPath(AbsolutePath = false, ParentFolder = "./", UseBackslashes = false)]
        [PropertyOrder(10)]
        public string spriteAtlasSaveDirPath;

        [Title("SpriteAtlas配置")]
        [InfoBox("只需要配置好纹理放置的目录以及生成方式即可，spriteatlas的文件名会自动生成")]
        [LabelText("spriteatlas文件数据")]
        [PropertyOrder(20)]
        [HideReferenceObjectPicker]
        [TableList(AlwaysExpanded = true, NumberOfItemsPerPage = 5, DrawScrollView = false, ShowPaging = true)]
        //[ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 10, AlwaysAddDefaultValue = true, CustomAddFunction = "AddSpriteAtlasItem")]
        public List<SpriteAtlasItemEditor> itemList = new List<SpriteAtlasItemEditor>();

        void AddSpriteAtlasItem()
        {
            itemList.Add(new SpriteAtlasItemEditor(null));
        }

        [Title("构建")]
        [InfoBox("刷新操作将完成以下操作：\r\n 根据「spriteatlas文件数据」创建或更新「spriteatlas保存目录」中的spriteatlas文件")]
        [Button("刷新所有的spriteatlas文件", ButtonSizes.Large)]
        [PropertyOrder(21)]
        void RefreshAll()
        {
            if (itemList.Count == 0)
            {
                return;
            }

            EditorUtility.DisplayProgressBar("进度", "spriteatlas文件刷新中...", 0);

            //检查保存目录是否存在，不在则生成
            if (!Directory.Exists(spriteAtlasSaveDirPath))
            {
                Directory.CreateDirectory(spriteAtlasSaveDirPath);
            }

            for (var i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                var progress = (i + 1f) / itemList.Count;
                EditorUtility.DisplayProgressBar("进度", "spriteatlas文件刷新中...", progress);

                SpriteAtlasToolsUtility.UpdateSpriteAtlas(item.ToSpriteAtlasItemVO());
            }

            EditorUtility.ClearProgressBar();
        }

        public struct SpriteAtlasItemEditor
        {
            [TableColumnWidth(60, false)]
            [Button("构建")]
            [PropertyOrder(-1)]
            void Refresh()
            {
                SpriteAtlasToolsUtility.UpdateSpriteAtlas(ToSpriteAtlasItemVO());
            }

            /// <summary>
            /// 是否子目录单独创建spriteatlas
            /// </summary>
            [TableColumnWidth(190, false)]
            [LabelText("子目录单独生成spriteatlas"), LabelWidth(150)]
            public bool texturesDirPath;

            /// <summary>
            /// 打包纹理集的目录
            /// </summary>
            [LabelText("纹理资源目录"), LabelWidth(80)]
            [HideLabel]
            [FolderPath(AbsolutePath = false, ParentFolder = "./", UseBackslashes = false, RequireExistingPath = true)]
            public string packingDirPath;

            public SpriteAtlasItemEditor(SpriteAtlasToolsConfigVO.SpriteAtlasItemVO vo)
            {
                if (null == vo)
                {
                    vo = new SpriteAtlasToolsConfigVO.SpriteAtlasItemVO();
                }

                this.packingDirPath = vo.texturesDirPath;
                this.texturesDirPath = vo.isSubDirSplit;
            }

            public SpriteAtlasToolsConfigVO.SpriteAtlasItemVO ToSpriteAtlasItemVO()
            {
                var vo = new SpriteAtlasToolsConfigVO.SpriteAtlasItemVO();
                vo.texturesDirPath = packingDirPath;
                vo.isSubDirSplit = texturesDirPath;
                return vo;
            }
        }
    }
}
