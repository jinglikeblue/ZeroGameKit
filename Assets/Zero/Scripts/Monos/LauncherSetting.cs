using UnityEngine;
using Zero;

/// <summary>
/// ��Ӧ��CustomEditor��LauncherSettingCustomEditor
/// </summary>
public class LauncherSetting : MonoBehaviour
{   
    public LauncherSettingData data;

    /// <summary>
    /// ����Resources�е�launcher_setting_data��������ȫ�µĶ���
    /// </summary>
    /// <returns></returns>
    static public LauncherSettingData LoadLauncherSettingDataFromResources()
    {
        var ta = Resources.Load<TextAsset>(ZeroConst.LAUNCHER_SETTING_NAME);
        if (null != ta)
        {
            return LitJson.JsonMapper.ToObject<LauncherSettingData>(ta.text);
        }
        return null;
    }

}
