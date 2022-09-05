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
    public class BuildResModule : AEditorModule
    {
        public BuildResModule(EditorWindow editorWin) : base(editorWin)
        {
        }

        [Title("热更资源打包", TitleAlignment = TitleAlignments.Centered)]
        [Title("勾选构建内容")]
        [LabelText("Copy Files (拷贝 @Files 文件夹到发布目录)"), ToggleLeft]
        public bool isCopyFiles = true;
        [LabelText("Build AssetBundles (构建AB包)"), ToggleLeft]
        public bool isBuildAB = true;
        [LabelText("Build DLL (构建热更代码)"), ToggleLeft]
        public bool isBuildDLL = true;
        [LabelText("Build res.json (构建资源版本号)"), ToggleLeft]
        public bool isBuildResJson = true;        

        [LabelText("发布热更资源"), Button(ButtonSizes.Large)]
        void BuildPart1()
        {
            if (isCopyFiles)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "开始拷贝Files资源文件夹", 0f);
                Debug.Log("开始拷贝Files资源文件夹");                
                CopyFiles();
                Debug.Log("Files资源文件夹拷贝完成");
            }

            if (isBuildAB)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "开始发布AssetBundle", 0f);
                Debug.Log("开始发布AssetBundle");
                //发布AB资源
                BuildAssetBundle();
                Debug.Log("AssetBundle发布完成");
            }

            if (isBuildDLL)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "正在发布DLL", 0f);
                Debug.Log("开始发布DLL");
                BuildDll(() =>
                {
                    Debug.Log("DLL发布成功");
                    BuildPart2();
                },
                () =>
                {
                    Debug.Log("DLL发布失败");
                    EditorUtility.ClearProgressBar();
                });
            }
            else
            {
                BuildPart2();
            }
        }



        [LabelText("发布完成后打开发布目录"), ToggleLeft, PropertyOrder(800)]
        [InlineButton("OpenPublishDir", "打开发布目录")]
        public bool isOpenPublishDir = true;

        [Space(50)]
        [Title("内嵌资源构建")]
        [LabelText("自动拷贝到内嵌资源目录(StreamingAssets/res)"), ToggleLeft, PropertyOrder(900)]
        [InlineButton("OpenBuiltinDir", "打开内嵌资源目录")]        
        public bool isCopyToBuiltinDir = false;

        [HorizontalGroup("BuiltinResCopy")]
        [LabelText("拷贝到内嵌资源目录"), Button(ButtonSizes.Large), PropertyOrder(901)]
        void CopyToBuiltinDir()
        {
            if (EditorUtility.DisplayDialog("确定窗口", "确定拷贝构建内容到'StreamingAssets/res'？", "是", "否"))
            {
                FileUtility.CopyDir(ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.STREAMING_ASSETS_RES_DATA_PATH);
                AssetDatabase.Refresh();
            }
        }

        [LabelText("Build StreamingAssets/res中的res.json"), Button(ButtonSizes.Large), PropertyOrder(901)]
        void BuildBuiltinResJson()
        {
            new ResJsonBuildCommand(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH).Execute();
        }

        [HorizontalGroup("BuiltinResCopy")]
        [LabelText("清空内嵌资源目录"), Button(ButtonSizes.Large), PropertyOrder(901)]
        void CleanBuiltinDir()
        {
            if (EditorUtility.DisplayDialog("确定窗口", "确定清空'StreamingAssets/res'目录？", "是", "否"))
            {
                if (Directory.Exists(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH))
                {
                    Directory.Delete(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH, true);
                    AssetDatabase.Refresh();
                }
            }
        }

        void OpenPublishDir()
        {
            //打开目录
            ZeroEditorUtil.OpenDirectory(ZeroConst.PUBLISH_RES_ROOT_DIR);
        }

        void BuildPart2()
        {
            if (isBuildResJson)
            {
                EditorUtility.DisplayProgressBar("打包热更资源", "开始发布版本描述文件", 0f);
                Debug.Log("开始发布版本描述文件");
                BuildResJsonFile();
                Debug.Log("版本描述文件发布完成");
            }

            if (isCopyToBuiltinDir)
            {
                EditorUtility.DisplayProgressBar("构建内嵌资源", "开始拷贝资源到内嵌目录", 0f);
                CopyToBuiltinDir();
            }

            if (isOpenPublishDir)
            {
                OpenPublishDir();                
            }
            else
            {
                EditorUtility.DisplayDialog("", "发布完成!", "确定");
            }

            EditorUtility.ClearProgressBar();
        }



        void OpenBuiltinDir()
        {
            //打开目录
            ZeroEditorUtil.OpenDirectory(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH);
        }



        void CopyFiles()
        {
            if (Directory.Exists(ZeroEditorConst.FILES_PUBLISH_DIR))
            {
                Directory.Delete(ZeroEditorConst.FILES_PUBLISH_DIR, true);
            }
            //拷贝文件
            FileUtility.CopyDir(ZeroConst.HOT_FILES_ROOT_DIR, ZeroEditorConst.FILES_PUBLISH_DIR, (s,t)=> {
                var ext = Path.GetExtension(s);
                if (ext.Equals(".meta"))
                {
                    return false;
                }
                return true;
            });
        }

        //private bool CheckCopyEnable(string sourceFile, string targetFile)
        //{
        //    if (Path.GetExtension(sourceFile).Equals(".meta"))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 构建热更AssetBundle资源
        /// </summary>
        void BuildAssetBundle()
        {
            //标记目标目录
            new AssetBundleBuildCommand(ZeroConst.HOT_RESOURCES_ROOT_DIR, ZeroEditorConst.ASSET_BUNDLE_PUBLISH_DIR).Execute();
        }

        /// <summary>
        /// 构建热更DLL文件
        /// </summary>
        void BuildDll(Action onBuildSuccess, Action onBuildFail)
        {
            var cmd = new DllBuildCommand(ZeroEditorConst.HOT_SCRIPT_ROOT_DIR, ZeroEditorConst.DLL_PUBLISH_DIR);
            cmd.onFinished += (DllBuildCommand self, bool isSuccess) => {
                if (isSuccess)
                {
                    //继续打包
                    onBuildSuccess?.Invoke();
                }
                else
                {
                    onBuildFail?.Invoke();
                }
            };
            cmd.Execute();
        }

        /// <summary>
        /// 构建res.json文件
        /// </summary>
        void BuildResJsonFile()
        {
            new ResJsonBuildCommand(ZeroConst.PUBLISH_RES_ROOT_DIR).Execute();
        }
    }
}
