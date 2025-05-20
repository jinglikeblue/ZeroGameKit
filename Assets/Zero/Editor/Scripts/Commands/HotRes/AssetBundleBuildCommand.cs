using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// AssetBundle打包指令
    /// </summary>
    class AssetBundleBuildCommand
    {
        /// <summary>
        /// 资源的根目录
        /// </summary>
        public readonly string resRootDir;

        /// <summary>
        /// 资源的输出
        /// </summary>
        public readonly string outputDir;

        /// <summary>
        /// 发布生成的Manifest文件
        /// </summary>
        public AssetBundleManifest assetBundleManifest { get; private set; }

        /// <summary>
        /// key：ab包名称   value：对应的资源列表
        /// </summary>
        Dictionary<string, List<string>> _abDic;

        /// <summary>
        /// key: ab包名称   value：ab包依赖的资源的Set集合
        /// </summary>
        Dictionary<string, HashSet<string>> _dependsDic;

        /// <summary>
        /// 附加AssetBundle中使用到的资源
        /// </summary>
        HashSet<string> _appendAssetsSet;


        public AssetBundleBuildCommand(string resRootDir, string outputDir)
        {
            this.resRootDir = FileUtility.CombineDirs(true, resRootDir);
            this.outputDir = outputDir;
            _abDic = new Dictionary<string, List<string>>();
            _dependsDic = new Dictionary<string, HashSet<string>>();
            _appendAssetsSet = new HashSet<string>();
        }

        public void Execute()
        {
            //找出所有要打包的资源
            FindAssetBundles();

            //附加要打包的资源
            AppendAssetBundles();

            //根据交叉引用算法优化AssetBundle
            CreateCrossAssetBundle();

            //打包前的资源检查
            CheckAssetBundles();

            //开始打包AssetBundle（打包到Library中的ZeroHotResCache中）
            BuildAssetBundlesToCacheDir();

            //从「ZeroHotResCache」中拷贝需要的文件到resDir中
            Move2ReleaseDir();

        }

        /// <summary>
        /// 找出所有要打包的资源
        /// </summary>
        private void FindAssetBundles()
        {
            string[] assetFileList = Directory.GetFiles(resRootDir, "*", SearchOption.AllDirectories);
            foreach (string assetFile in assetFileList)
            {
                if (Path.GetExtension(assetFile).Equals(".meta"))
                {
                    continue;
                }

                var temp = FileUtility.StandardizeBackslashSeparator(assetFile);
                AssetImporter ai = AssetImporter.GetAtPath(temp);

                if (null == ai)
                {
                    continue;
                }

                //根据资源的路径分AB包                 
                string assetPath = ai.assetPath.Replace(resRootDir, "");
                //根据资源所在文件夹，计算AssetBundle文件名
                string abName = Path.GetDirectoryName(assetPath);                
                if (string.IsNullOrEmpty(abName))
                {
                    //资源直接在根目录下
                    abName = ZeroConst.ROOT_AB_FILE_NAME;
                }
                else
                {
                    abName = FileUtility.StandardizeBackslashSeparator(abName);
                }

                //加上后缀名
                abName += ZeroConst.AB_EXTENSION;

                GetAssetList(abName).Add(ai.assetPath);
                //找出依赖资源
                FindDepends(ai.assetPath, GetDependsSet(abName));
            }
        }

        /// <summary>
        /// 附加要打包的资源。
        /// 对于没有（或无法）放置在@ab下的资源，通过接口添加到AssetBundles打包清单中
        /// </summary>
        private void AppendAssetBundles()
        {
            if (false == ZeroEditorSettings.ASSET_BUNDLE_APPENDER_ENABLE)
            {
                return;
            }

            //AssetBundle 打包扩展
            var appenderTypes = TypeUtility.FindSubclasses(typeof(BaseAssetBundleAppender));
            if(null != appenderTypes && appenderTypes.Length > 0)
            {
                //遍历每一个派生类
                foreach(var type in appenderTypes)
                {
                    BaseAssetBundleAppender appender = Activator.CreateInstance(type) as BaseAssetBundleAppender;
                    var assetBundleBuilds = appender.AssetBundles();

                    //遍历每一个派生类的接口返回的AssetBundleBuild对象数组
                    foreach(var abb in assetBundleBuilds)
                    {
                        var abName = abb.assetBundleName + ZeroConst.AB_EXTENSION;
                        var assetList = GetAssetList(abName);
                        foreach(var assetPath in abb.assetNames)
                        {
                            //如果指定的资源是resRootDir下的，就不用处理
                            if (assetPath.StartsWith(resRootDir))
                            {
                                Debug.LogWarning($"附加的AssetBundle中，不应该存在[{resRootDir}]目录下的资源: {assetPath} !!! 已自动忽略。");
                                continue;
                            }

                            if (_appendAssetsSet.Contains(assetPath))
                            {
                                throw new Exception($"资源对象不能同时打包到多个AssetBundle中： [{assetPath}]");
                            }

                            _appendAssetsSet.Add(assetPath);

                            assetList.Add(assetPath);
                            //找出依赖的资源
                            FindDepends(assetPath, GetDependsSet(abName));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 找出资源依赖的资源（如果依赖的资源已标记为AB，则忽略）
        /// </summary>
        /// <param name="ai"></param>
        void FindDepends(string assetPath, HashSet<string> dependsSet)
        {
            //获取依赖的资源
            string[] dps = AssetDatabase.GetDependencies(assetPath);
            foreach (string dependPath in dps)
            {
                if (dependPath.StartsWith(resRootDir) || dependPath.Contains(".cs"))
                {
                    //要过滤掉依赖的@ab目录中的文件和脚本文件，@ab目录中的文件已设置，而脚本不能打包
                    continue;
                }

                //依赖的资源
                if (false == dependsSet.Contains(dependPath))
                {
                    dependsSet.Add(dependPath);
                }
            }
        }

        List<string> GetAssetList(string abName)
        {
            if (false == _abDic.ContainsKey(abName))
            {
                _abDic[abName] = new List<string>();
            }

            return _abDic[abName];
        }

        HashSet<string> GetDependsSet(string abName)
        {
            if (false == _dependsDic.ContainsKey(abName))
            {
                _dependsDic[abName] = new HashSet<string>();
            }

            return _dependsDic[abName];
        }

        /// <summary>
        /// 创建交叉文件资源。最佳化计算，依赖的ab包的数量会增多，但是保持的是项目的最小颗粒化依赖
        /// </summary>
        void CreateCrossAssetBundle()
        {
            if (false == ZeroEditorSettings.CREATE_CROSS_ASSET_BUNDLE_ENABLE)
            {
                return;
            }

            #region 找出每一个资源依赖它的AB集合Set
            Dictionary<string, HashSet<string>> asset2ABDic = new Dictionary<string, HashSet<string>>();
            foreach (var ab in _dependsDic)
            {
                foreach (var asset in ab.Value)
                {
                    if (_appendAssetsSet.Contains(asset))
                    {
                        //如果这个资源是附加AssetBundle中添加的资源，则忽略掉依赖处理
                        continue;
                    }

                    if (false == asset2ABDic.ContainsKey(asset))
                    {
                        asset2ABDic[asset] = new HashSet<string>();
                    }
                    asset2ABDic[asset].Add(ab.Key);
                }
            }
            #endregion

            #region 移除掉只被一个AB依赖的资源，这种资源只需要和唯一依赖它的AB一起打包就行了
            HashSet<string> toRemoveKeySet = new HashSet<string>();
            foreach (var pair in asset2ABDic)
            {
                if (pair.Value.Count <= 1)
                {
                    toRemoveKeySet.Add(pair.Key);
                }
            }

            foreach (var toRemoveKey in toRemoveKeySet)
            {
                asset2ABDic.Remove(toRemoveKey);
            }
            #endregion

            #region 遍历每一个资源，找到和它被同样AB集合的资源，打到一个依赖AB包中，依次命名为cross_0, cross_1, cross_2
            int i = 0;
            //标记已处理的资源
            HashSet<string> usedAsset = new HashSet<string>();
            foreach (var assetPair in asset2ABDic)
            {
                if (usedAsset.Contains(assetPair.Key))
                {
                    continue;
                }
                string abName = string.Format("auto_depends/cross_{0}{1}", i++, ZeroConst.AB_EXTENSION);
                List<string> assetList = new List<string>();
                assetList.Add(assetPair.Key);
                //标记为已使用
                usedAsset.Add(assetPair.Key);
                foreach (var assetPair1 in asset2ABDic)
                {
                    if (usedAsset.Contains(assetPair1.Key))
                    {
                        continue;
                    }

                    //首先判断被AB依赖的数量是否一致
                    if (assetPair.Value.Count == assetPair1.Value.Count)
                    {
                        bool isSame = true;
                        //判断是否所有的AB都一样
                        foreach (var tempABName in assetPair.Value)
                        {
                            if (false == assetPair1.Value.Contains(tempABName))
                            {
                                isSame = false;
                                break;
                            }
                        }

                        if (isSame)
                        {
                            assetList.Add(assetPair1.Key);
                            usedAsset.Add(assetPair1.Key);
                        }
                    }
                }

                _abDic[abName] = assetList;
            }
            #endregion
        }

        /// <summary>
        /// 打包前的资源检查
        /// </summary>
        void CheckAssetBundles()
        {
            #region 场景检查，场景文件不能和其它资源在同一个AB里面
            
            foreach (var abb in _abDic)
            {
                int scenesCount = 0;
                int assetsCount = 0; 
                
                foreach (var filePath in abb.Value)
                {
                    if (filePath.EndsWith(".unity"))
                    {
                        scenesCount++;
                    }
                    else
                    {
                        assetsCount++;
                    }
                }
                
                if (scenesCount > 0 && assetsCount > 0)
                {
                    var folder = FileUtility.CombinePaths(ZeroConst.PROJECT_AB_DIR, Path.GetFileNameWithoutExtension(abb.Key));
                    throw new Exception($"场景文件不能和其它资源在同一个AB（文件夹）里面，请检查文件夹: {folder}");
                }
            }

            #endregion

        }

        /// <summary>
        /// 首先打包AssetBundle到缓存发布目录
        /// </summary>
        void BuildAssetBundlesToCacheDir()
        {
            AssetBundleBuild[] abbList = new AssetBundleBuild[_abDic.Count];
            int i = 0;
            foreach (var abb in _abDic)
            {
                abbList[i] = new AssetBundleBuild();
                abbList[i].assetBundleName = abb.Key;
                abbList[i].assetNames = abb.Value.ToArray();
                i++;
            }

            if (false == Directory.Exists(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR))
            {
                Directory.CreateDirectory(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR);
            }
            
            assetBundleManifest = BuildPipeline.BuildAssetBundles(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, abbList, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);

            if (null != assetBundleManifest)
            {
                //清理不再需要的资源(需要修改算法，只保留需要的，完全清理不需要的资源)
                CleanCahceDir();

                #region 生成一个依赖文件表
                var filePath = FileUtility.CombinePaths(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, "dependencies.json");
                HotResEditorUtility.CreateAssetBundleDependenciesJson(assetBundleManifest, filePath);
                Debug.Log($"生成的依赖文件查找表: {filePath}");
                #endregion
            }
            else
            {
                throw new Exception("AssetBundle打包失败!");
            }
        }

        void CleanCahceDir()
        {            
            var assetBundles = assetBundleManifest.GetAllAssetBundles();
            //需要留下的文件
            HashSet<string> usefulFileSet = new HashSet<string>();
            usefulFileSet.Add(ZeroConst.AB_DIR_NAME);
            usefulFileSet.Add(ZeroConst.AB_DIR_NAME + ".manifest");
            foreach (var abFile in assetBundles)
            {
                usefulFileSet.Add(abFile);
                usefulFileSet.Add(abFile + ".manifest");
            }

            var abDir = FileUtility.CombineDirs(true, ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR);
            var files = Directory.GetFiles(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var tempFile = FileUtility.StandardizeBackslashSeparator(file);
                var filePath = tempFile.Replace(abDir, "");
                if (false == usefulFileSet.Contains(filePath))
                {
                    //不需要的AB
                    File.Delete(tempFile);
                    Debug.LogFormat("删除文件：" + tempFile);
                }
            }

            //删除空的文件夹
            DirectoryInfo dir = new DirectoryInfo(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR);
            DirectoryInfo[] subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (DirectoryInfo subdir in subdirs)
            {
                if (false == Directory.Exists(subdir.FullName))
                {
                    continue;
                }
                FileInfo[] subFiles = subdir.GetFiles("*", SearchOption.AllDirectories);
                if (subFiles.Length == 0)
                {
                    subdir.Delete(true);
                    Debug.LogFormat("删除文件夹：" + FileUtility.StandardizeBackslashSeparator(subdir.FullName));
                }
            }

            AssetBundle.UnloadAllAssetBundles(true);
        }

        void Move2ReleaseDir()
        {
            //清空发布目录
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }

            //移动AB文件
            var assetBundles = assetBundleManifest.GetAllAssetBundles();
            foreach (var ab in assetBundles)
            {
                string sourceABPath = FileUtility.CombinePaths(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, ab);
                string targetABPath = FileUtility.CombinePaths(outputDir, ab);
                FileUtility.CopyFile(sourceABPath, targetABPath, true);
            }

            //移动Manifest文件
            string sourceManifestPath = FileUtility.CombinePaths(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, ZeroConst.AB_DIR_NAME);
            string targetManifestPath = FileUtility.CombinePaths(outputDir, ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            FileUtility.CopyFile(sourceManifestPath, targetManifestPath, true);

            //#region 打印这个Manifest文件的内容
            //{
            //    var ab = AssetBundle.LoadFromFile(targetManifestPath);
            //    var manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //    var filePath = FileUtility.CombinePaths(ZeroEditorConst.ASSET_BUNDLE_CACHE_DIR, "dependencies_ab.json");
            //    AssetBundleUtility.CreateDependenciesJson(manifest, filePath);
            //    Debug.Log($"生成的依赖文件查找表: {filePath}");
            //    ab.Unload(true);
            //}
            //#endregion
        }
    }    
}