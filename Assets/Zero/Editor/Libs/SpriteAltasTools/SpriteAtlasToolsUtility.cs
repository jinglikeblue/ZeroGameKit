using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace ZeroEditor
{
    /// <summary>
    /// SpriteAtlasTools辅助工具
    /// 关联类: SpriteAtlasExtensions,SpriteAtlas    
    /// </summary>
    class SpriteAtlasToolsUtility
    {
        public static event Action onAddSpriteAtlas;

        public static event Action onBuildAll;

        public const string CONFIG_NAME = "sprite_atlas_tools_config.json";

        static public string GenerateSpriteAtlasNameByPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return null;
            }

            const string ROOT_DIR = "Assets/";

            path = FileUtility.StandardizeBackslashSeparator(path);
            path = path.Remove(0, ROOT_DIR.Length);
            var name = path.Replace("/", "_").ToLower();
            return name + ".spriteatlas";
        }

        /// <summary>
        /// 添加一个目录到SpriteAtlas配置
        /// </summary>
        /// <param name="texturesDirPath"></param>
        /// <param name="isSubDirSplit"></param>
        static public void AddSpriteAtlas(string texturesDirPath, bool isSubDirSplit)
        {
            var cfg = EditorConfigUtil.LoadConfig<SpriteAtlasToolsConfigVO>(CONFIG_NAME);

            var itemVO = new SpriteAtlasToolsConfigVO.SpriteAtlasItemVO();
            itemVO.texturesDirPath = texturesDirPath;
            itemVO.isSubDirSplit = isSubDirSplit;
            cfg.itemList.Add(itemVO);

            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);

            onAddSpriteAtlas?.Invoke();
        }

        /// <summary>
        /// 构建所有的SpriteAtlas
        /// </summary>
        static public void BuildAll()
        {
            var cfg = EditorConfigUtil.LoadConfig<SpriteAtlasToolsConfigVO>(CONFIG_NAME);
            for (var i = 0; i < cfg.itemList.Count; i++)
            {
                var item = cfg.itemList[i];
                EditorUtility.DisplayProgressBar("进度", "spriteatlas文件构建中...", 0);
                BuildSpriteAtlas(cfg.spriteAtlasSaveDirPath, item, cfg.packingTextureWidthLimit, cfg.packingTextureHeightLimit);
            }
            EditorUtility.ClearProgressBar();

            onBuildAll?.Invoke();
        }

        static public void BuildSpriteAtlas(string spriteAtlasDir, SpriteAtlasToolsConfigVO.SpriteAtlasItemVO vo, int limiteWidth, int limitHeight)
        {
            CreateOrUpdateSpriteAtlas(spriteAtlasDir, vo.texturesDirPath, vo.isSubDirSplit, limiteWidth, limitHeight);
        }

        static void CreateOrUpdateSpriteAtlas(string spriteAtlasDir, string texturesDirPath, bool isSubDirSplit, int limiteWidth, int limitHeight)
        {
            var name = GenerateSpriteAtlasNameByPath(texturesDirPath);
            var filePath = FileUtility.CombinePaths(spriteAtlasDir, name);
            string[] files;
            if (isSubDirSplit)
            {
                files = Directory.GetFiles(texturesDirPath);
                var dirs = Directory.GetDirectories(texturesDirPath);
                foreach(var dir in dirs)
                {
                    //是文件夹，拆开创建子spriteatlas
                    CreateOrUpdateSpriteAtlas(spriteAtlasDir, dir, isSubDirSplit, limiteWidth, limitHeight);
                }
            }
            else
            {
                files = Directory.GetFiles(texturesDirPath, "*", SearchOption.AllDirectories);
            }                     

            var spriteList = new List<Sprite>();
            foreach (var file in files)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
                if (null == sprite)
                {
                    continue;
                }

                //尺寸限制
                if(sprite.texture.width >= limiteWidth || sprite.texture.height >= limitHeight)
                {
                    continue;
                }                
                
                spriteList.Add(sprite);
            }

            bool isEmpty = spriteList.Count == 0 ? true : false;

            var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);
            if (null == sa)
            {
                if (isEmpty)
                {
                    //SpriteAtlas还未创建，列表又是空的，就不创建了。
                    //PS:已创建的不删除，是可能有自定义的配置，需要保留
                    return;
                }

                var dirPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var packingSettings = new SpriteAtlasPackingSettings();
                packingSettings.enableRotation = false;
                packingSettings.enableTightPacking = false;
                packingSettings.padding = 2;

                sa = new SpriteAtlas();
                sa.SetPackingSettings(packingSettings);
                AssetDatabase.CreateAsset(sa, filePath);
            }

            var oldSpriteList = sa.GetPackables();
            sa.Remove(oldSpriteList);
            sa.Add(spriteList.ToArray());

            AssetDatabase.SaveAssets();
        }
    }
}