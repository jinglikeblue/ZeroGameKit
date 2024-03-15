using System.Collections;
using System.Collections.Generic;
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
}
