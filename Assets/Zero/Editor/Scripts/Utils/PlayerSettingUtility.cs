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
    /// 检查指定宏是否存在
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
    /// 获取项目已设置的宏
    /// </summary>
    /// <returns></returns>
    static public string[] GetScriptingDefineSymbols(BuildTargetGroup targetGroup)
    {
        var sourceStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        string[] defines = sourceStr.Split(';');
        return defines;
    }

    /// <summary>
    /// 添加代码编译宏
    /// </summary>
    /// <param name="define">宏定义字符串</param>
    /// <param name="targetGroup">不设置的话，会使用当前编辑器对应打包平台</param>    
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
    static public bool RemoveScriptingDefineSymbols(string define, BuildTargetGroup targetGroup)
    {
        string[] defines = GetScriptingDefineSymbols(targetGroup);

        var defineList = defines.ToList();
        defineList.Remove(define);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, ComboScriptingDefineSymbols(defineList));

        return true;
    }
}
