using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using Zero;

/// <summary>
/// URP管线相关资源
/// </summary>
class ShaderAssetBundleAppender : BaseAssetBundleAppender
{
    public override AssetBundleBuild[] AssetBundles()
    {
        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = "appends/shaders";
        abb.assetNames = new string[] {
        "Assets/URP/SVC.shadervariants",        
        "Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader",
        "Assets/Zero/Shaders/UI/TextOutline.shader"
        };
        return new AssetBundleBuild[] { abb };
    }
}
