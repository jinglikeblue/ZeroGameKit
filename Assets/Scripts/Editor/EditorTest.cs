using System;
using System.Reflection;
using Jing;
using UnityEditor;
using UnityEngine;
using Zero;

public class EditorTest
{
    [MenuItem("Test/Editor/FindSubclasses")]
    public static void FindSubclassesTest()
    {
        var results = TypeUtility.FindSubclasses(typeof(BaseAssetBundleAppender));
        Debug.Log($"[FindSubclassesTest] 查找结果:{results?.Length}");
    }

    [MenuItem("Test/打印平台名称")]
    public static void PrintPlatformNames()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        var name = BuildPipeline.GetBuildTargetName(buildTarget);
        Debug.Log($"当前平台名称:{name}, Appplication.platform:{Application.platform}");


        foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
        {
            // 跳过废弃或无效的平台
            if (target == BuildTarget.NoTarget)
                continue;

            // 检查是否支持该平台
            var isSupport = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Unknown, target);
            var targetName = BuildPipeline.GetBuildTargetName(target);

            // 获取 BuildTarget 枚举的 FieldInfo
            FieldInfo fieldInfo = typeof(BuildTarget).GetField(target.ToString());

            // 检查是否带有 [Obsolete] 特性
            bool isObsolete = Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute));

            if (isSupport)
            {
                Debug.Log($"平台:{targetName} 是否支持:{isSupport} 是否废弃:{isObsolete}");
            }
        }
    }
}