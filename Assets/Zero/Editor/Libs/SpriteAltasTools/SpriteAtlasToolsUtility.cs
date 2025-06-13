using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace ZeroEditor
{
    /// <summary>
    /// SpriteAtlasTools辅助工具
    /// 关联类: SpriteAtlasExtensions,SpriteAtlas    
    /// </summary>
    static class SpriteAtlasToolsUtility
    {
        public static event Action onAddSpriteAtlas;

        public static event Action onBuildAll;

        public const string CONFIG_NAME = "sprite_atlas_tools_config.json";

        /// <summary>
        /// 是否使用的是SpriteAtlasV2模式
        /// </summary>
        public static bool IsSpriteAtlasV2Mode => EditorSettings.spritePackerMode == SpritePackerMode.SpriteAtlasV2;

        /// <summary>
        /// 默认的纹理集设置
        /// </summary>
        private static readonly SpriteAtlasPackingSettings DefaultPackingSettings = new SpriteAtlasPackingSettings()
        {
            enableRotation = false,
            enableTightPacking = false,
            padding = 2,
        };


        public static string GenerateSpriteAtlasNameByPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return null;
            }

            const string ROOT_DIR = "Assets/";

            path = FileUtility.StandardizeBackslashSeparator(path);
            path = path.Remove(0, ROOT_DIR.Length);
            var name = path.Replace("/", "_").ToLower();

            var ext = ".spriteatlas";
            if (IsSpriteAtlasV2Mode)
            {
                ext += "v2";
            }

            return name + ext;
        }

        /// <summary>
        /// 添加一个目录到SpriteAtlas配置
        /// </summary>
        /// <param name="texturesDirPath"></param>
        /// <param name="isSubDirSplit"></param>
        static public void AddSpriteAtlas(string texturesDirPath, bool isSubDirSplit)
        {
            var cfg = EditorConfigUtil.LoadConfig<SpriteAtlasToolsConfigVO>(CONFIG_NAME);
            foreach (var item in cfg.itemList)
            {
                if (item.texturesDirPath == texturesDirPath)
                {
                    Debug.Log("失败：添加到SpriteAtlas配置的目录已存在");
                    return;
                }
            }

            var itemVO = new SpriteAtlasToolsConfigVO.SpriteAtlasItemVO();
            itemVO.texturesDirPath = texturesDirPath;
            itemVO.isSubDirSplit = isSubDirSplit;
            cfg.itemList.Add(itemVO);

            EditorConfigUtil.SaveConfig(cfg, CONFIG_NAME);

            Debug.Log("成功：添加目录到SpriteAtlas配置");

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

        public static void BuildSpriteAtlas(string spriteAtlasDir, SpriteAtlasToolsConfigVO.SpriteAtlasItemVO vo, int limiteWidth, int limitHeight)
        {
            CreateOrUpdateSpriteAtlas(spriteAtlasDir, vo.texturesDirPath, vo.isSubDirSplit, limiteWidth, limitHeight);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
                foreach (var dir in dirs)
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
                if (sprite.texture.width >= limiteWidth || sprite.texture.height >= limitHeight)
                {
                    continue;
                }

                spriteList.Add(sprite);
            }

            try
            {
                if (IsSpriteAtlasV2Mode)
                {
                    TryUpdateSpriteAtlasV2(filePath, spriteList.ToArray());
                }
                else
                {
                    TryUpdateSpriteAtlasV1(filePath, spriteList.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SpriteAtlas] 失败: {filePath}");
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 尝试更新SpriteAtlasV1
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sprites"></param>
        static void TryUpdateSpriteAtlasV1(string filePath, Object[] sprites)
        {
            if (null == sprites || 0 == sprites.Length)
            {
                return;
            }

            var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);
            if (null == sa)
            {
                sa = new SpriteAtlas();
                sa.SetPackingSettings(DefaultPackingSettings);
                AssetDatabase.CreateAsset(sa, filePath);
            }

            // var oldSpriteList = sa.GetPackables();
            // sa.Remove(oldSpriteList);

            ClearPackingList(sa);
            sa.Add(sprites);
        }

        /// <summary>
        /// 尝试更新SpriteAtlasV2
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sprites"></param>
        static void TryUpdateSpriteAtlasV2(string filePath, Object[] sprites)
        {
            if (0 == sprites.Length)
            {
                return;
            }

            SpriteAtlasAsset saa = SpriteAtlasAsset.Load(filePath);
            if (null == saa)
            {
                saa = new SpriteAtlasAsset();
                saa.SetPackingSettings(DefaultPackingSettings);
            }
            else
            {
                ClearPackingList(saa);
                // var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);
                // var oldSpriteList = sa.GetPackables();
                // saa.Remove(oldSpriteList);
            }

            saa.Add(sprites);

            SpriteAtlasAsset.Save(saa, filePath);
        }

        [MenuItem("Test/SpriteAtlasClearList")]
        private static void TestSO()
        {
            var filePath = "Assets/Examples/SpriteAtlas/examples_art_gui.spriteatlas";
            Debug.Log($"SpriteAtlasClearList");
            var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);
            // var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlasAsset>("Assets/Examples/SpriteAtlas/examples_art_gui.spriteatlasv2");
            ClearPackingList(sa);
            // SpriteAtlasAsset.Save(sa, filePath);
            // EditorUtility.SetDirty(sa);
            AssetDatabase.SaveAssets(); // 确保资源文件被写入磁盘
            AssetDatabase.Refresh(); // 刷新资源数据库
        }

        /// <summary>
        /// 清空Packables数组
        /// </summary>
        /// <param name="obj"></param>
        private static void ClearPackingList(Object obj)
        {
            SerializedObject so = new SerializedObject(obj);
            //先尝试查找V2版本的数据
            var packables = so.FindProperty("m_ImporterData.packables");
            if (null == packables)
            {
                //如果失败，则查找V1版本的数据
                packables = so.FindProperty("m_EditorData.packables");
            }
            
            if (null != packables)
            {
                // Debug.Log($"Name:{packables.name}, Type:{packables.propertyType}, Size:{packables.arraySize}");
                packables.ClearArray();
            }
            else
            {
                // Debug.Log($"Not Found: packables");
            }

            //保存修改到磁盘
            so.ApplyModifiedProperties(); // 关键步骤：应用修改
        }

        /// <summary>
        /// 刷新预览
        /// </summary>
        public static void PackPreview(string spriteAtlasDir)
        {
            var list = new List<SpriteAtlas>();
            var files = Directory.GetFiles(spriteAtlasDir);
            foreach (var file in files)
            {
                var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(file);
                if (sa != null)
                {
                    list.Add(sa);
                }
            }

            SpriteAtlasUtility.PackAtlases(list.ToArray(), ZeroEditorConst.BUILD_PLATFORM);
        }

        /// <summary>
        /// 清理Library下的AtlasCache文件夹
        /// </summary>
        public static void ClearLibraryCache()
        {
            var di = new DirectoryInfo(FileUtility.CombinePaths(ZeroEditorConst.PROJECT_PATH, "Library", "AtlasCache"));
            if (di.Exists)
            {
                di.Delete(true);
            }
            Debug.Log($"清理Editor下的SpriteAtlas缓存，完成！");
        }
    }
}