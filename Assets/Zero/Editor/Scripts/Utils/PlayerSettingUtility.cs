using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// PlayerSetting���ø�������
/// </summary>
public static class PlayerSettingUtility
{
    /// <summary>
    /// ��Ӵ�������
    /// </summary>
    /// <param name="define">�궨���ַ���</param>
    /// <param name="targetGroup">�����õĻ�����ʹ�õ�ǰ�༭����Ӧ���ƽ̨</param>    
    static public bool AddScriptingDefineSymbols(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Unknown)
    {
        if(targetGroup == BuildTargetGroup.Unknown)
        {
            if(BuildTargetGroup.Unknown == EditorUserBuildSettings.selectedBuildTargetGroup)
            {
                return false;
            }

            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        }        
        
        var sourceStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        string[] defines = sourceStr.Split(';');

        if (defines.Contains(define) == false)
        {
            var defineList = defines.ToList();
            defineList.Add(define);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, ComboScriptingDefineSymbols(defineList));
        }

        return true;
    }

    /// <summary>
    /// �ϲ���������
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static string ComboScriptingDefineSymbols(List<string> list)
    {
        if(list.Count == 0)
        {
            return "";
        }

        string str = list[0];
        for(int i = 1; i < list.Count; i++)
        {
            str += $";{list[i]}";
        }
        return str;
    }

    /// <summary>
    /// �Ƴ���������
    /// </summary>
    /// <param name="define">�궨���ַ���</param>
    /// <param name="targetGroup">�����õĻ�����ʹ�õ�ǰ�༭����Ӧ���ƽ̨</param>
    static public bool RemoveScriptingDefineSymbols(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Unknown)
    {
        if (targetGroup == BuildTargetGroup.Unknown)
        {
            if (BuildTargetGroup.Unknown == EditorUserBuildSettings.selectedBuildTargetGroup)
            {
                return false;
            }

            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        }       
        
        var sourceStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        string[] defines = sourceStr.Split(';');

        var defineList = defines.ToList();
        defineList.Remove(define);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, ComboScriptingDefineSymbols(defineList));

        return true;
    }
}
