using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// PlayerSetting设置辅助工具
/// </summary>
public static class PlayerSettingUtility
{
    /// <summary>
    /// 添加代码编译宏
    /// </summary>
    /// <param name="define">宏定义字符串</param>
    /// <param name="targetGroup">不设置的话，会使用当前编辑器对应打包平台</param>    
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
    /// 合并代码编译宏
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
    /// 移除代码编译宏
    /// </summary>
    /// <param name="define">宏定义字符串</param>
    /// <param name="targetGroup">不设置的话，会使用当前编辑器对应打包平台</param>
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
