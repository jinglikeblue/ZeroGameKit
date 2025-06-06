using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Build后的处理内容
    /// </summary>
    public class PostprocessBuildMsg : IPostprocessBuildWithReport
    {
        public int callbackOrder => int.MaxValue;

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log(LogColor.Zero2("[Zero][Build][PostprocessBuild] Build后处理"));
        }
    }
}