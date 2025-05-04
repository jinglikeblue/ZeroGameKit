using Sirenix.OdinInspector;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 对应的CustomEditor：LauncherSettingCustomEditor
    /// </summary>
    public class LauncherSetting : MonoBehaviour
    {
        public LauncherSettingData data;

        /// <summary>
        /// 加载Resources中的launcher_setting_data，并返回全新的对象
        /// </summary>
        /// <returns></returns>
        public static LauncherSettingData LoadLauncherSettingDataFromResources()
        {
            var ta = Resources.Load<TextAsset>(ZeroConst.LAUNCHER_SETTING_NAME);
            if (null != ta)
            {
                return Json.ToObject<LauncherSettingData>(ta.text);
            }
            return null;
        }
    }
}