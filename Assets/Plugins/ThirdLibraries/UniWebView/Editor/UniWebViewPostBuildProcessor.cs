
// #if UNITY_2023_2_OR_NEWER
#if UNIWEBVIEW_NEW_ANDROID_BUILD_SYSTEM

using System;
using System.Linq;
using Unity.Android.Gradle;
using Unity.Android.Gradle.Manifest;
using UnityEditor.Android;
using UnityEngine;
using Action = Unity.Android.Gradle.Manifest.Action;

class UniWebViewPostBuildModifier : AndroidProjectFilesModifier {
    
    const string ContextSettingsKey = "uniwebview_settings";
    
    public override AndroidProjectFilesModifierContext Setup() {
        var context = new AndroidProjectFilesModifierContext {
            Dependencies = {
                DependencyFiles = new[] {
                    // Set UniWebView editor settings asset path to be included in the build. With that we do not need
                    // a clean build to update the settings anymore (maybe, not confirmed).
                    UniWebViewEditorSettings.AssetPath
                }
            }
        };

        var settings = UniWebViewEditorSettings.GetOrCreateSettings();
        context.SetData(ContextSettingsKey, settings);
        return context;
    }

    public override void OnModifyAndroidProjectFiles(AndroidProjectFiles projectFiles) {
        var settings = projectFiles.GetData<UniWebViewEditorSettingsReading>(ContextSettingsKey);
        PatchUnityLibraryAndroidManifest(projectFiles.UnityLibraryManifest, settings);
        PatchUnityLibraryBuildGradle(projectFiles.UnityLibraryBuildGradle, settings);
        PatchGradleProperty(projectFiles.GradleProperties, settings);
    }

    private void PatchUnityLibraryAndroidManifest(
        AndroidManifestFile manifest, UniWebViewEditorSettingsReading settings
    ) {
        // Set hardwareAccelerated
        var launcherActivities = manifest.Manifest.GetActivitiesWithLauncherIntent();
        foreach (var activity in launcherActivities)  {
            // Required for playing video in web view.
            activity.Attributes.HardwareAccelerated.Set(true);
        }
        
        // Set usesCleartextTraffic
        if (settings.usesCleartextTraffic) {
            manifest.Manifest.Application.Attributes.UsesCleartextTraffic.Set(true);
        }
        
        // Set WRITE_EXTERNAL_STORAGE permission
        if (settings.writeExternalStorage) {
            AddUsesPermission(manifest, "android.permission.WRITE_EXTERNAL_STORAGE");
        }

        // Set ACCESS_FINE_LOCATION permission
        if (settings.accessFineLocation) {
            AddUsesPermission(manifest, "android.permission.ACCESS_FINE_LOCATION");
        }
        
        // Set auth callback intent filter
        if (settings.authCallbackUrls.Length > 0 || settings.supportLINELogin) {
            var authActivity = new Activity();
            authActivity.Attributes.Name.Set("com.onevcat.uniwebview.UniWebViewAuthenticationActivity");
            authActivity.Attributes.Exported.Set(true);
            authActivity.Attributes.LaunchMode.Set(LaunchMode.SingleTask);
            authActivity.Attributes.ConfigChanges.Set(
                new[] {
                    ConfigChanges.Orientation, ConfigChanges.ScreenSize, ConfigChanges.KeyboardHidden
                });
            
            foreach (var url in settings.authCallbackUrls) {
                AddAuthCallbacksIntentFilter(authActivity, url);
            }
            if (settings.supportLINELogin) {
                AddAuthCallbacksIntentFilter(authActivity, "lineauth://auth");
            }
            manifest.Manifest.Application.ActivityList.AddElement(authActivity);            
        }
    }

    private void PatchUnityLibraryBuildGradle(ModuleBuildGradleFile gradleFile, UniWebViewEditorSettingsReading settings) {
        if (settings.addsKotlin) {
            var kotlinPrefix = "org.jetbrains.kotlin:kotlin-stdlib-jdk7:";
            var kotlinVersion = String.IsNullOrWhiteSpace(settings.kotlinVersion) 
                ? UniWebViewEditorSettings.defaultKotlinVersion : settings.kotlinVersion;
            ReplaceContentOrAddStartsWith(gradleFile.Dependencies, 
                kotlinPrefix, 
                kotlinVersion);
            Debug.Log("<UniWebView> Updated Kotlin dependency in build.gradle.");
        }

        if (settings.addsAndroidBrowser) {
            var browserPrefix = "androidx.browser:browser:";
            var browserVersion = String.IsNullOrWhiteSpace(settings.androidBrowserVersion) 
                ? UniWebViewEditorSettings.defaultAndroidBrowserVersion : settings.androidBrowserVersion;
            ReplaceContentOrAddStartsWith(gradleFile.Dependencies,
                browserPrefix, 
                browserVersion);
            Debug.Log("<UniWebView> Updated Browser dependency in build.gradle.");
        }

        if (!settings.addsAndroidBrowser && settings.addsAndroidXCore) {
            var androidXCorePrefix = "androidx.core:core:";
            var androidXCoreVersion = String.IsNullOrWhiteSpace(settings.androidXCoreVersion) 
                ? UniWebViewEditorSettings.defaultAndroidXCoreVersion : settings.androidXCoreVersion;
            ReplaceContentOrAddStartsWith(gradleFile.Dependencies,
                androidXCorePrefix, 
                androidXCoreVersion);
            Debug.Log("<UniWebView> Updated Android X Core dependency in build.gradle.");
        }
    }

    private void PatchGradleProperty(GradlePropertiesFile file, UniWebViewEditorSettingsReading settings) {
        var values = file.GetElements();
        foreach (var ele in values) {
            if (ele.GetRaw().Contains("android.enableJetifier")) {
                ele.SetRaw("android.enableJetifier=" + (settings.enableJetifier ? "true": "false"));
            }
        }
    }

    private void AddUsesPermission(AndroidManifestFile manifest, string name) {

        var list = manifest.Manifest.UsesPermissionList;
        foreach (var item in list) {
            Debug.LogError(item.GetName());
        }
        
        var existing = manifest.Manifest.UsesPermissionList
            .FirstOrDefault(ele => ele.Attributes.Name.Get() == name);
        if (existing != null) {
            return;
        }
        var permission = new UsesPermission();
        permission.Attributes.Name.Set(name);
        manifest.Manifest.UsesPermissionList.AddElement(permission);
    }

    private void AddAuthCallbacksIntentFilter(Activity activity, string callbackUrl) {
        var uri = new Uri(callbackUrl);
        var scheme = uri.Scheme;
        
        if (String.IsNullOrEmpty(scheme)) {
            Debug.LogError("<UniWebView> Auth callback url contains an empty scheme. Please check the url: " + callbackUrl);
            return;
        }
        
        var intentFilter = new IntentFilter();
        
        var action = new Action();
        action.Attributes.Name.Set("android.intent.action.VIEW");
        intentFilter.ActionList.AddElement(action);
        
        var defaultCategory = new Category();
        intentFilter.CategoryList.AddElement(defaultCategory);
        defaultCategory.Attributes.Name.Set("android.intent.category.DEFAULT");

        var browsableCategory = new Category();
        browsableCategory.Attributes.Name.Set("android.intent.category.BROWSABLE");
        intentFilter.CategoryList.AddElement(browsableCategory);

        var data = new Data();
        data.Attributes.Scheme.Set(scheme);
        if (!String.IsNullOrEmpty(uri.Host)) {
            data.Attributes.Host.Set(uri.Host);
        }
        if (uri.Port != -1) {
            data.Attributes.Port.Set(uri.Port.ToString());
        }
        if (!String.IsNullOrEmpty(uri.PathAndQuery) && uri.PathAndQuery != "/") {
            data.Attributes.Path.Set(uri.PathAndQuery);
        }
        intentFilter.DataList.AddElement(data);
        activity.IntentFilterList.AddElement(intentFilter);
    }

    private void ReplaceContentOrAddStartsWith(Dependencies dependencies, string prefix, string version) {
        var all = dependencies.GetElements();
        var matching = "implementation '" + prefix;
        var found = all.FirstOrDefault(ele => ele.GetRaw().StartsWith(matching));
        if (found != null) {
            found.SetRaw($"implementation '{prefix}{version}'");
        } else {
            dependencies.AddDependencyImplementationRaw($"'{prefix}{version}'");
        }
    }
}

#else
using System;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System.IO;
using System.Text;

class UniWebViewPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 1; } }
    public void OnPostGenerateGradleAndroidProject(string path) {
        Debug.Log("<UniWebView> UniWebView Post Build Scirpt is patching manifest file and gradle file...");
        PatchAndroidManifest(path);
        PatchBuildGradle(path);
        PatchGradleProperty(path);
    }

    private void PatchAndroidManifest(string root) {
        var manifestFilePath = GetManifestFilePath(root);
        var manifest = new UniWebViewAndroidManifest(manifestFilePath);
        
        var changed = false;
        
        Debug.Log("<UniWebView> Set hardware accelerated to enable smooth web view experience and HTML5 support like video and canvas.");
        changed = manifest.SetHardwareAccelerated() || changed;

        var settings = UniWebViewEditorSettings.GetOrCreateSettings();
        if (settings.usesCleartextTraffic) {
            changed = manifest.SetUsesCleartextTraffic() || changed;
        }
        if (settings.writeExternalStorage) {
            changed = manifest.AddWriteExternalStoragePermission() || changed;
        }
        if (settings.accessFineLocation) {
            changed = manifest.AddAccessFineLocationPermission() || changed;
        }
        if (settings.authCallbackUrls.Length > 0) {
            changed = manifest.AddAuthCallbacksIntentFilter(settings.authCallbackUrls) || changed;
        }

        if (settings.supportLINELogin) {
            changed = manifest.AddAuthCallbacksIntentFilter(new string[] { "lineauth://auth" }) || changed;
        }

        if (changed) {
            manifest.Save();
        }
    }

    private void PatchBuildGradle(string root) {
        var gradleFilePath = GetGradleFilePath(root);
        var config = new UniWebViewGradleConfig(gradleFilePath);

        var settings = UniWebViewEditorSettings.GetOrCreateSettings();
        
        var kotlinPrefix = "implementation 'org.jetbrains.kotlin:kotlin-stdlib-jdk7:";
        var kotlinVersion = String.IsNullOrWhiteSpace(settings.kotlinVersion) 
            ? UniWebViewEditorSettings.defaultKotlinVersion : settings.kotlinVersion;

        var browserPrefix = "implementation 'androidx.browser:browser:";
        var browserVersion = String.IsNullOrWhiteSpace(settings.androidBrowserVersion) 
            ? UniWebViewEditorSettings.defaultAndroidBrowserVersion : settings.androidBrowserVersion;

        var androidXCorePrefix = "implementation 'androidx.core:core:";
        var androidXCoreVersion = String.IsNullOrWhiteSpace(settings.androidXCoreVersion) 
            ? UniWebViewEditorSettings.defaultAndroidXCoreVersion : settings.androidXCoreVersion;
        
        var dependenciesNode = config.Root.FindChildNodeByName("dependencies");
        if (dependenciesNode != null) {
            // Add kotlin
            if (settings.addsKotlin) {
                dependenciesNode.ReplaceContentOrAddStartsWith(kotlinPrefix, kotlinPrefix + kotlinVersion + "'");
                Debug.Log("<UniWebView> Updated Kotlin dependency in build.gradle.");
            }

            // Add browser package
            if (settings.addsAndroidBrowser) {
                dependenciesNode.ReplaceContentOrAddStartsWith(browserPrefix, browserPrefix + browserVersion + "'");
                Debug.Log("<UniWebView> Updated Browser dependency in build.gradle.");
            }

            // Add Android X Core package
            if (!settings.addsAndroidBrowser && settings.addsAndroidXCore) {
                // When adding android browser to the project, we don't need to add Android X Core package, since gradle resolves for it.
                dependenciesNode.ReplaceContentOrAddStartsWith(androidXCorePrefix, androidXCorePrefix + androidXCoreVersion + "'");
                Debug.Log("<UniWebView> Updated Android X Core dependency in build.gradle.");
            }
        } else {
            Debug.LogError("UniWebViewPostBuildProcessor didn't find the `dependencies` field in build.gradle.");
            Debug.LogError("Although we can continue to add a `dependencies`, make sure you have setup Gradle and the template correctly.");

            var newNode = new UniWebViewGradleNode("dependencies", config.Root);
            if (settings.addsKotlin) {
                newNode.AppendContentNode(kotlinPrefix + kotlinVersion + "'");
            }
            if (settings.addsAndroidBrowser) {
                newNode.AppendContentNode(browserPrefix + browserVersion + "'");
            }

            if (settings.addsAndroidXCore) {
                newNode.AppendContentNode(androidXCorePrefix + androidXCoreVersion + "'");
            }
            newNode.AppendContentNode("implementation(name: 'UniWebView', ext:'aar')");
            config.Root.AppendChildNode(newNode);
        }
        config.Save();
    }

    private void PatchGradleProperty(string root) {
        var gradlePropertyFilePath = GetGradlePropertyFilePath(root);
        var patcher =
            new UniWebViewGradlePropertyPatcher(gradlePropertyFilePath, UniWebViewEditorSettings.GetOrCreateSettings());
        patcher.Patch();
    }

    private string CombinePaths(string[] paths) {
        var path = "";
        foreach (var item in paths) {
            path = Path.Combine(path, item);
        }
        return path;
    }

    private string GetManifestFilePath(string root) {
        string[] comps = {root, "src", "main", "AndroidManifest.xml"};
        return CombinePaths(comps);
    }

    private string GetGradleFilePath(string root) {
        string[] comps = {root, "build.gradle"};
        return CombinePaths(comps);
    }

    private string GetGradlePropertyFilePath(string root) {
        #if UNITY_2019_3_OR_NEWER
        string[] compos = {root, "..", "gradle.properties"};
        #else
        string[] compos = {root, "gradle.properties"};
        #endif
        return CombinePaths(compos);
    }
}
#endif