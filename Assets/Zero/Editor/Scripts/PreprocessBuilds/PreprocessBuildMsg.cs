using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// Build前的预处理内容
    /// </summary>
    public class PreprocessBuildMsg : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return int.MinValue; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("[PreprocessBuild] Build Will Start");
        }
    }
}