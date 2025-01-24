using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class UniWebViewEditorSettings: ScriptableObject
{
    private const string AssetPath = "Assets/Editor/UniWebView/settings.asset";

    [SerializeField]
    internal bool usesCleartextTraffic = false;

    [SerializeField]
    internal bool writeExternalStorage = false;

    [SerializeField]
    internal bool accessFineLocation = false;

    [SerializeField]
    internal bool addsKotlin = true;

    [SerializeField] 
    internal string kotlinVersion = null;

    [SerializeField]
    internal bool addsAndroidBrowser = true;

    [SerializeField]
    internal string androidBrowserVersion = null;
    
    [SerializeField]
    internal bool addsAndroidXCore = false;
    
    [SerializeField]
    internal string androidXCoreVersion = null;

    [SerializeField]
    internal bool enableJetifier = true;

    [SerializeField]
    internal string[] authCallbackUrls = { };
    
    [SerializeField]
    internal bool supportLINELogin = false;

    internal static string defaultKotlinVersion = "1.6.21";
    internal static string defaultAndroidBrowserVersion = "1.2.0";
    internal static string defaultAndroidXCoreVersion = "1.5.0";

    internal static UniWebViewEditorSettings GetOrCreateSettings() {
        var settings = AssetDatabase.LoadAssetAtPath<UniWebViewEditorSettings>(AssetPath);

        if (settings == null) {
            settings = ScriptableObject.CreateInstance<UniWebViewEditorSettings>();

            Directory.CreateDirectory("Assets/Editor/UniWebView/");
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
        }

        return settings;
    }

    internal static SerializedObject GetSerializedSettings() {
        return new SerializedObject(GetOrCreateSettings());
    }
}

// UniWebViewEditorSettings is not working well with AndroidProjectFilesModifier.
// (reading it requires main thread, but the OnModifyAndroidProjectFiles is not in main thread)
[Serializable]
public class UniWebViewEditorSettingsReading {
    public bool usesCleartextTraffic = false;
    public bool writeExternalStorage = false;
    public bool accessFineLocation = false;
    public bool addsKotlin = true;
    public string kotlinVersion = null;
    public bool addsAndroidBrowser = true;
    public string androidBrowserVersion = null;
    public bool addsAndroidXCore = false;
    public string androidXCoreVersion = null;
    public bool enableJetifier = true;
    public string[] authCallbackUrls = { };
    public bool supportLINELogin = false;
}

static class UniWebViewSettingsProvider {
    static SerializedObject settings;

    #if UNITY_2018_3_OR_NEWER
    private class Provider : SettingsProvider {
        public Provider(string path, SettingsScope scope = SettingsScope.User): base(path, scope) {}
        public override void OnGUI(string searchContext) {
            DrawPref();
        }
    }
    [SettingsProvider]
    static SettingsProvider UniWebViewPref() {
        return new Provider("Preferences/UniWebView");
    }
    #else
    [PreferenceItem("UniWebView")]
    #endif
    static void DrawPref() {
        EditorGUIUtility.labelWidth = 320;
        EditorGUIUtility.fieldWidth = 20;
        if (settings == null) {
            settings = UniWebViewEditorSettings.GetSerializedSettings();
        }
        settings.Update();
        EditorGUI.BeginChangeCheck();

        // Manifest
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Android Manifest", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(settings.FindProperty("usesCleartextTraffic"));
        DrawDetailLabel("If you need to load plain HTTP content.");
        
        EditorGUILayout.PropertyField(settings.FindProperty("writeExternalStorage"));
        DrawDetailLabel("If you need to download an image from web page.");

        EditorGUILayout.PropertyField(settings.FindProperty("accessFineLocation"));
        DrawDetailLabel("If you need to enable location support in web view.");
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        // Gradle
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Gradle Build", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(settings.FindProperty("addsKotlin"));
        DrawDetailLabel("Turn off this if another library is already adding Kotlin runtime.");
        var addingKotlin = settings.FindProperty("addsKotlin").boolValue;
        if (addingKotlin) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(settings.FindProperty("kotlinVersion"), GUILayout.Width(400));
            DrawDetailLabel("If not specified, use the default version: " + UniWebViewEditorSettings.defaultKotlinVersion);
            EditorGUI.indentLevel--;            
        }

        EditorGUILayout.PropertyField(settings.FindProperty("addsAndroidBrowser"));
        DrawDetailLabel("Turn off this if another library is already adding 'androidx.browser:browser'.");
        var addingBrowser = settings.FindProperty("addsAndroidBrowser").boolValue;
        if (addingBrowser) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(settings.FindProperty("androidBrowserVersion"), GUILayout.Width(400));
            DrawDetailLabel("If not specified, use the default version: " + UniWebViewEditorSettings.defaultAndroidBrowserVersion);
            EditorGUI.indentLevel--;            
        }

        if (!addingBrowser) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("UniWebView at least requires `androidx.core` to run. Without it, your game will crash when launching.\nIf you do not have another `androidx.core` package in the project, enable the option below.", MessageType.Warning);
            EditorGUILayout.PropertyField(settings.FindProperty("addsAndroidXCore"));
            DrawDetailLabel("Turn on this if you disabled `Adds Android Browser` and there is no other library adding 'androidx.core:core'.");
            var addingCore = settings.FindProperty("addsAndroidXCore").boolValue;
            if (addingCore) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(settings.FindProperty("androidXCoreVersion"), GUILayout.Width(400));
                DrawDetailLabel("If not specified, use the default version: " + UniWebViewEditorSettings.defaultAndroidXCoreVersion);
                EditorGUI.indentLevel--;            
            }
            EditorGUILayout.EndVertical();
        }
        
        
        EditorGUILayout.PropertyField(settings.FindProperty("enableJetifier"));
        DrawDetailLabel("Turn off this if you do not need Jetifier (for converting other legacy support dependencies to Android X).");
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        // Auth callbacks
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Auth Callbacks", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(settings.FindProperty("authCallbackUrls"), true);
        DrawDetailLabel("Adds all available auth callback URLs here to use UniWebView's auth support.");
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(settings.FindProperty("supportLINELogin"));
        DrawDetailLabel("LINE Login is using a custom fixed scheme. If you want to support LINE Login, turn on this.");
        
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        EditorGUILayout.HelpBox("Read the help page to know more about all UniWebView preferences detail.", MessageType.Info);
        
        var style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.blue;
        if (GUILayout.Button("Help Page", style)) {
          Application.OpenURL("https://docs.uniwebview.com/guide/installation.html#optional-steps");
        }
        
        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck()) {
            settings.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
        EditorGUIUtility.labelWidth = 0;
    }

    static void DrawDetailLabel(string text) {
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField(text, EditorStyles.miniLabel);
        EditorGUI.indentLevel--;
    }
}