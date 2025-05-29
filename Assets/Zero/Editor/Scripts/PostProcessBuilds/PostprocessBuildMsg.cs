using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// Build后的处理内容
    /// </summary>
    public class PostprocessBuildMsg : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return int.MaxValue; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log("[Zero][Build][PostprocessBuild] Build后处理");
        }
    }
}