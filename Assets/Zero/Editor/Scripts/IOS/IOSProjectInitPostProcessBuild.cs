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
            if (false == _cfg.isEnable)
            {
                return;
            }
            _pbxProjectPath = PBXProject.GetPBXProjectPath(_xcodeProjectPath);
            _pbx = new PBXProject();
            _pbx.ReadFromString(File.ReadAllText(_pbxProjectPath));

            //UnityMain
            _mainGuid = _pbx.GetUnityMainTargetGuid();
            //UnityFramework
            _frameworkGuid = _pbx.GetUnityFrameworkTargetGuid();

            CopyAppIconSets();
            CopyFiles();
            ConfigurePBXProject();
            ConfigureFrameworksToOptional();
            ConfigureInfoPList();
            ConfigureCapability();
        }

        /// <summary>
        /// 添加文件到编译项
        /// </summary>
        /// <param name="vo">拷贝文件的信息</param>
        /// <param name="relativePath">文件的相对路径</param>
        void AddFileToBuild(IOSProjectInitConfigVO.CopyInfoVO vo)
        {
            var fileGuid = _pbx.AddFile(vo.toPath, vo.toPath, PBXSourceTree.Source);
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
        /// 拷贝图标集合
        /// </summary>
        void CopyAppIconSets()
        {
            if(_cfg.appIconSetList.Length == 0)
            {
                return;
            }

            foreach(var appIconSet in _cfg.appIconSetList)
            {
                var name = Path.GetFileName(appIconSet);
                var targetPath = FileUtility.CombineDirs(true, _xcodeProjectPath, "Unity-iPhone/Images.xcassets" , name);
                FileUtility.CopyDir(appIconSet, targetPath);
            }

            //将工程Build Settings中 Include all app icon assets改为 YES
            _pbx.SetBuildProperty(_mainGuid, "ASSETCATALOG_COMPILER_INCLUDE_ALL_APPICON_ASSETS", "YES");
        }

        /// <summary>
        /// 拷贝文件到项目
        /// </summary>
        /// <param name="cfg"></param>
        void CopyFiles()
        {
            var unityProjectPath = Directory.GetParent(Application.dataPath).FullName;
            var xcodeProjectPath = _xcodeProjectPath;
            foreach (var vo in _cfg.copyInfoList)
            {
                var sourcePath = FileUtility.CombinePaths(unityProjectPath, vo.fromPath);
                var targetPath = FileUtility.CombinePaths(xcodeProjectPath, vo.toPath);

                var type = FileUtility.CheckPathType(sourcePath);

                if (type == FileUtility.EPathType.OTHER)
                {
                    Debug.LogError($"IOSProjectInitPostProcessBuild:路径'{sourcePath}'有问题，无法进行复制操作");
                    continue;
                }

                switch (type)
                {
                    case FileUtility.EPathType.FILE:
                        FileUtility.CopyFile(sourcePath, targetPath, true);
                        AddFileToBuild(vo);
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
                                AddFileToBuild(vo);
                            }
                        );
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
                pbx.AddFrameworkToProject(targetGuid, framework.name, framework.isWeak);
            }

            if (vo.frameworksToOptionalList.Length > 0)
            {
                EditPBXProjFileToAddFrameworkWeak(_pbxProjectPath, vo.frameworksToOptionalList);
            }

            foreach (var entry in vo.file2BuildList)
            {
                pbx.AddFileToBuild(targetGuid, pbx.AddFile(entry.Key, entry.Value, PBXSourceTree.Sdk));
            }

            foreach (var entry in vo.toSetBuildPropertyList)
            {
                var key = entry.Key;
                var value = entry.Value;
                pbx.SetBuildProperty(targetGuid, key, value);
                Debug.Log($"SetBuildProperty [targetGuid = {targetGuid}] [key = {key}] [value = {value}]");
            }

            foreach (var entry in vo.toAddBuildPropertyList)
            {
                var key = entry.Key;
                var value = entry.Value;
                pbx.AddBuildProperty(targetGuid, key, value);
                Debug.Log($"AddBuildProperty [targetGuid = {targetGuid}] [key = {key}] [value = {value}]");
            }
        }

        void ConfigureFrameworksToOptional()
        {
            if (_cfg.main.frameworksToOptionalList.Length > 0)
            {
                EditPBXProjFileToAddFrameworkWeak(_pbxProjectPath, _cfg.main.frameworksToOptionalList);
            }

            if (_cfg.framework.frameworksToOptionalList.Length > 0)
            {
                EditPBXProjFileToAddFrameworkWeak(_pbxProjectPath, _cfg.framework.frameworksToOptionalList);
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

            foreach(var item in _cfg.pListAdvancedDataList)
            {
                pListEditor.Add(item);
            }

            pListEditor.Save();
        }

        void ConfigureCapability()
        {
            if (string.IsNullOrEmpty(_cfg.capabilitySetting.entitlementFilePath) || false == _cfg.capabilitySetting.entitlementFilePath.EndsWith(".entitlements"))
            {
                return;
            }

            //将工程Build Settings 的 Code Signing Entitlements 设置为文件路径
            _pbx.SetBuildProperty(_mainGuid, "CODE_SIGN_ENTITLEMENTS", _cfg.capabilitySetting.entitlementFilePath);

            var pcm = new ProjectCapabilityManager(_pbxProjectPath, _cfg.capabilitySetting.entitlementFilePath, "Unity-iPhone");

            if (_cfg.capabilitySetting.inAppPurchase)
            {
                pcm.AddInAppPurchase();
            }

            if (_cfg.capabilitySetting.pushNotifications.enable)
            {
                pcm.AddPushNotifications(_cfg.capabilitySetting.pushNotifications.development);
            }

            if (_cfg.capabilitySetting.backgroundModes != BackgroundModesOptions.None)
            {
                pcm.AddBackgroundModes(_cfg.capabilitySetting.backgroundModes);
            }

            if (_cfg.capabilitySetting.associatedDomains.Length > 0)
            {
                pcm.AddAssociatedDomains(_cfg.capabilitySetting.associatedDomains);
            }

            if (_cfg.capabilitySetting.signInWithApple)
            {
                pcm.AddSignInWithApple();
            }

            if (_cfg.capabilitySetting.accessWiFiInformation)
            {
                pcm.AddAccessWiFiInformation();
            }

            if (_cfg.capabilitySetting.gameCener)
            {
                pcm.AddGameCenter();
            }

            pcm.WriteToFile();
        }

        /// <summary>
        /// 修改第三方Framework Weak->Optional
        /// </summary>
        public static void EditPBXProjFileToAddFrameworkWeak(string path, string[] frameworkNames)
        {
            const string INSERT_STR = " platformFilter = ios; settings = {ATTRIBUTES = (Weak, ); };";
            const string SEARCH_FLAG_0_FORMAT = "{0}.framework in Frameworks";
            const string SEARCH_FLAG_1 = "= {isa = PBXBuildFile; fileRef =";
            const string LINE_END_FLAG = " };";
            var lines = File.ReadAllLines(path);
            foreach (var name in frameworkNames)
            {
                var frameworkName = name;
                if (frameworkName.EndsWith(".framework"))
                {
                    frameworkName = frameworkName.Replace(".framework", "");
                }
                var searchFlag0 = string.Format(SEARCH_FLAG_0_FORMAT, frameworkName);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.Contains(searchFlag0) && line.Contains(SEARCH_FLAG_1) && !line.Contains(INSERT_STR))
                    {
                        Debug.Log($"{name} 的Status改为Optional");
                        var insertIndex = line.LastIndexOf(LINE_END_FLAG);
                        var newLine = line.Insert(insertIndex, INSERT_STR);
                        lines[i] = newLine;
                    }
                }
            }
            File.WriteAllLines(path, lines);
        }
    }
}
#endif