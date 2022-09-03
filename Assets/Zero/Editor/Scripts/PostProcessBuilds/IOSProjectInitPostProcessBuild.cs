#if UNITY_IPHONE
using Jing;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace ZeroEditor.IOS
{
    /// <summary>
    /// XCODE项目的初始化
    /// </summary>
    public class IOSProjectInitPostProcessBuild
    {
        /// <summary>
        /// XCODE项目发布后的处理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (BuildTarget.iOS != target)
            {
                return;
            }

            new IOSProjectInitPostProcessBuild(path).Process();
        }

        IOSProjectInitConfigVO _cfg;

        string _xcodeProjectPath;

        PBXProject _pbx;

        string _mainGuid;

        string _frameworkGuid;

        string _pbxProjectPath;

        IOSProjectInitPostProcessBuild(string path)
        {
            this._xcodeProjectPath = path;
        }

        public void Process()
        {
            _cfg = EditorConfigUtil.LoadConfig<IOSProjectInitConfigVO>(IOSInfoplistInitModule.CONFIG_NAME);
            _pbxProjectPath = PBXProject.GetPBXProjectPath(_xcodeProjectPath);
            _pbx = new PBXProject();
            _pbx.ReadFromString(File.ReadAllText(_pbxProjectPath));

            //UnityMain
            _mainGuid = _pbx.GetUnityMainTargetGuid();
            //UnityFramework
            _frameworkGuid = _pbx.GetUnityFrameworkTargetGuid();

            CopyFiles();
            ConfigurePBXProject();
            ConfigureInfoPList();            
        }

        /// <summary>
        /// 添加文件到编译项
        /// </summary>
        /// <param name="vo">拷贝文件的信息</param>
        /// <param name="relativePath">文件的相对路径</param>
        void AddFileToBuild(IOSProjectInitConfigVO.CopyInfoVO vo, string absolutePath)
        {
            var fileGuid = _pbx.AddFile(absolutePath, vo.toPath, PBXSourceTree.Source);
            if (vo.isAddToMain)
            {
                _pbx.AddFileToBuild(_mainGuid, fileGuid);
            }
            if (vo.isAddToFramework)
            {
                _pbx.AddFileToBuild(_frameworkGuid, fileGuid);
            }
        }

        /// <summary>
        /// 拷贝文件到项目
        /// </summary>
        /// <param name="cfg"></param>
        void CopyFiles()
        { 
            var unityProjectPath = ZeroEditorConst.PROJECT_PATH;
            var xcodeProjectPath = _xcodeProjectPath;
            foreach (var vo in _cfg.copyInfoList)
            {
                var sourcePath = FileUtility.CombinePaths(unityProjectPath, vo.fromPath);
                var targetPath = FileUtility.CombinePaths(xcodeProjectPath, vo.toPath);

                var type = FileUtility.CheckPathType(sourcePath);
                switch (type)
                {
                    case FileUtility.EPathType.FILE:
                        FileUtility.CopyFile(sourcePath, targetPath, true);
                        AddFileToBuild(vo, targetPath);
                        break;
                    case FileUtility.EPathType.DIRECTORY:
                        FileUtility.CopyDir(sourcePath, targetPath,
                            (string sourceFile, string targetFile) =>
                            {
                                //不拷贝Unity生成的meta文件
                                if (sourcePath.EndsWith("*.meta"))
                                {
                                    return false;
                                }
                                return true;
                            },
                            (string targetFile) =>
                            {
                                //已拷贝完
                                AddFileToBuild(vo, targetFile);
                            }
                        );
                        break;
                    default:
                        Debug.LogError($"IOSProjectInitPostProcessBuild:路径'{sourcePath}'有问题，无法进行复制操作");
                        break;
                }

                //将文件或目录，添加到【Build Phases】的【Copy Bundle Resources】中
                if (vo.isAddPathToResourcesBuildPhase)
                {
                    string targetGuid = null;
                    string fileGuid = _pbx.FindFileGuidByProjectPath(vo.toPath);
                    if (vo.isAddToMain)
                    {
                        targetGuid = _mainGuid;
                    }
                    else if (vo.isAddToFramework)
                    {
                        targetGuid = _frameworkGuid;
                    }
                    _pbx.AddFileToBuildSection(targetGuid, _pbx.GetResourcesBuildPhaseByTarget(targetGuid), fileGuid);
                }
            }
        }

        void ConfigurePBXProject()
        {
            //UnityMain
            ConfigurePBXProject(_pbx, _mainGuid, _cfg.main);

            //UnityFramework
            ConfigurePBXProject(_pbx, _frameworkGuid, _cfg.framework);

            File.WriteAllText(_pbxProjectPath, _pbx.WriteToString());
        }

        void ConfigurePBXProject(PBXProject pbx, string targetGuid, IOSProjectInitConfigVO.PBXProjectSettingVO vo)
        {
            foreach (var framework in vo.frameworkToProjectList)
            {
                pbx.AddFrameworkToProject(targetGuid, framework, false);
            }

            foreach (var entry in vo.file2BuildList)
            {
                pbx.AddFileToBuild(targetGuid, pbx.AddFile(entry.Key, entry.Value, PBXSourceTree.Sdk));
            }

            foreach (var entry in vo.buildPropertyList)
            {
                pbx.SetBuildProperty(targetGuid, entry.Key, entry.Value);
            }
        }

        void ConfigureInfoPList()
        {
            string plistPath = _xcodeProjectPath + "/Info.plist";
            InfoPListEditor pListEditor = new InfoPListEditor(plistPath);

            foreach (var entry in _cfg.pListDataList)
            {
                pListEditor.Add(entry.Key, entry.Value);
            }

            foreach (string urlScheme in _cfg.urlSchemeList)
            {
                pListEditor.AddUrlScheme("ZeroUrlSchemes", urlScheme);
            }

            foreach (string whiteUrlScheme in _cfg.appQueriesSchemeList)
            {
                pListEditor.AddLSApplicationQueriesScheme(whiteUrlScheme);
            }
            pListEditor.Save();
        }
    }
}
#endif