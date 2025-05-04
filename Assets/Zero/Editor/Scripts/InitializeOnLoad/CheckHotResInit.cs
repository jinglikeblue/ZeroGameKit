using Jing;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 检查热更资源初始化配置，如果项目没有res.json和setting.json是无法运行的，所以没有的话会默认生成
    /// </summary>
    [InitializeOnLoad]
    public class CheckHotResInit
    {        
        static CheckHotResInit()
        {
            //检查setting.json文件，不存在则生成
            var settingFilePath = FileUtility.CombinePaths(ZeroEditorConst.PUBLISH_RES_ROOT_DIR, ZeroConst.SETTING_FILE_NAME);
            if (!File.Exists(settingFilePath))
            {
                var fi = new FileInfo(settingFilePath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                Debug.Log(LogColor.Blue($"初始化「setting.json」文件：{settingFilePath}"));
                var cfg = new SettingVO();
                string jsonStr = Json.ToJsonIndented(cfg);
                File.WriteAllText(settingFilePath, jsonStr);
            }


            //检查res.json文件，不存在则生成
            var resFilePath = FileUtility.CombinePaths(ZeroEditorConst.PUBLISH_RES_ROOT_DIR, ZeroConst.RES_JSON_FILE_NAME);
            if (!File.Exists(resFilePath))
            {
                var fi = new FileInfo(resFilePath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                Debug.Log(LogColor.Blue($"初始化「res.json」文件：{resFilePath}"));
                new ResJsonBuildCommand(ZeroEditorConst.PUBLISH_RES_ROOT_DIR).Execute();
            }
        }
    }
}