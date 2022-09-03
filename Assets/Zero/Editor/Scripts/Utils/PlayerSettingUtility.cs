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
    /// ���ָ�����Ƿ����
    /// </summary>
    /// <param name="define"></param>
    /// <param name="targetGroup"></param>
    /// <returns></returns>
    static public bool CheckScriptingDefineSymbolsExist(string define, BuildTargetGroup targetGroup)
    {
        var defines = GetScriptingDefineSymbols(targetGroup);
        return defines.Contains(define);
    }

    /// <summary>
    /// ��ȡ��Ŀ�����õĺ�
    /// </summary>
    /// <returns></returns>
    static public string[] GetScriptingDefineSymbols(BuildTargetGroup targetGroup)
    {
        var sourceStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        string[] defines = sourceStr.Split(';');
        return defines;
    }

    /// <summary>
    /// ��Ӵ�������
    /// </summary>
    /// <param name="define">�궨���ַ���</param>
    /// <param name="targetGroup">�����õĻ�����ʹ�õ�ǰ�༭����Ӧ���ƽ̨</param>    
    static public bool AddScriptingDefineSymbols(string define, BuildTargetGroup targetGroup)
    {
        string[] defines = GetScriptingDefineSymbols(targetGroup);

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
    static public bool RemoveScriptingDefineSymbols(string define, BuildTargetGroup targetGroup)
    {
        string[] defines = GetScriptingDefineSymbols(targetGroup);

        var defineList = defines.ToList();
        defineList.Remove(define);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, ComboScriptingDefineSymbols(defineList));

        return true;
    }
}
