using Jing;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// ����ȸ���Դ��ʼ�����ã������Ŀû��res.json��setting.json���޷����еģ�����û�еĻ���Ĭ������
    /// </summary>
    [InitializeOnLoad]
    public class CheckHotResInit
    {        
        static CheckHotResInit()
        {
            //���setting.json�ļ���������������
            var settingFilePath = FileUtility.CombinePaths(ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.SETTING_FILE_NAME);
            if (!File.Exists(settingFilePath))
            {
                var fi = new FileInfo(settingFilePath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                Debug.Log(Log.Blue($"��ʼ����setting.json���ļ���{settingFilePath}"));
                var cfg = new SettingVO();
                string jsonStr = LitJson.JsonMapper.ToPrettyJson(cfg);
                File.WriteAllText(settingFilePath, jsonStr);
            }


            //���res.json�ļ���������������
            var resFilePath = FileUtility.CombinePaths(ZeroConst.PUBLISH_RES_ROOT_DIR, ZeroConst.RES_JSON_FILE_NAME);
            if (!File.Exists(resFilePath))
            {
                var fi = new FileInfo(resFilePath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                Debug.Log(Log.Blue($"��ʼ����res.json���ļ���{resFilePath}"));
                new ResJsonBuildCommand(ZeroConst.PUBLISH_RES_ROOT_DIR).Execute();
            }
        }
    }
}