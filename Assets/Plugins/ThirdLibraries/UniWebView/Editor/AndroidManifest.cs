using System.Xml;
using System.Text;
using System;
using UnityEngine;

internal class UniWebViewAndroidXmlDocument : XmlDocument {
    private readonly string path;
    protected readonly XmlNamespaceManager nameSpaceManager;
    protected const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

    protected UniWebViewAndroidXmlDocument(string path) {
        this.path = path;
        using (var reader = new XmlTextReader(path)) {
            reader.Read();
            Load(reader);
        }
        nameSpaceManager = new XmlNamespaceManager(NameTable);
        nameSpaceManager.AddNamespace("android", AndroidXmlNamespace);
    }

    public void Save() {
        SaveAs(path);
    }

    private void SaveAs(string path)
    {
        using var writer = new XmlTextWriter(path, new UTF8Encoding(false));
        writer.Formatting = Formatting.Indented;
        Save(writer);
    }
}

internal class UniWebViewAndroidManifest : UniWebViewAndroidXmlDocument {
    private readonly XmlElement manifestElement;
    private readonly XmlElement applicationElement;

    public UniWebViewAndroidManifest(string path) : base(path) {
        manifestElement = SelectSingleNode("/manifest") as XmlElement;
        applicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value) {
        XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    internal XmlNode GetActivityWithLaunchIntent() {
        return
            SelectSingleNode(
                "/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and "
                + "intent-filter/category/@android:name='android.intent.category.LAUNCHER']",
                nameSpaceManager);
    }

    internal bool SetUsesCleartextTraffic() {
        var changed = false;
        if (applicationElement.GetAttribute("usesCleartextTraffic", AndroidXmlNamespace) != "true") {
            applicationElement.SetAttribute("usesCleartextTraffic", AndroidXmlNamespace, "true");
            changed = true;
        }
        return changed;
    }

    internal bool SetHardwareAccelerated() {
        var changed = false;
        var activity = GetActivityWithLaunchIntent() as XmlElement;
        if (activity == null)
        {
            Debug.LogError(
                "There is no launch intent activity in the AndroidManifest.xml." +
                " Please check your AndroidManifest.xml file and make sure it has a main activity with intent filter");
            return false;
        }
        if (activity.GetAttribute("hardwareAccelerated", AndroidXmlNamespace) != "true") {
            activity.SetAttribute("hardwareAccelerated", AndroidXmlNamespace, "true");
            changed = true;
        }
        return changed;
    }

    internal bool AddCameraPermission() {
        var changed = false;
        var cameraPermission = "/manifest/uses-permission[@android:name='android.permission.CAMERA']";
        var cameraPermissionNode = SelectNodes(cameraPermission, nameSpaceManager);
        if (cameraPermissionNode == null || cameraPermissionNode.Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.CAMERA"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        
        var hardwareCamera = "/manifest/uses-feature[@android:name='android.hardware.camera']";
        var hardwareCameraNode = SelectNodes(hardwareCamera, nameSpaceManager);
        if (hardwareCameraNode == null || hardwareCameraNode.Count == 0) {
            var elem = CreateElement("uses-feature");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.hardware.camera"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddMicrophonePermission() {
        bool changed = false;
        var microphonePermission = "/manifest/uses-permission[@android:name='android.permission.MICROPHONE']";
        var microphonePermissionNode = SelectNodes(microphonePermission, nameSpaceManager);
        if (microphonePermissionNode == null || microphonePermissionNode.Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.MICROPHONE"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        
        var microphoneHardware = "/manifest/uses-feature[@android:name='android.hardware.microphone']";
        var microphoneHardwareNode = SelectNodes(microphoneHardware, nameSpaceManager);
        if (microphoneHardwareNode == null || microphoneHardwareNode.Count == 0) {
            var elem = CreateElement("uses-feature");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.hardware.microphone"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddReadExternalStoragePermission() {
        var changed = false;
        var externalPermission = "/manifest/uses-permission[@android:name='android.permission.READ_EXTERNAL_STORAGE']";
        var externalNode = SelectNodes(externalPermission, nameSpaceManager);
        if (externalNode == null || externalNode.Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.READ_EXTERNAL_STORAGE"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddWriteExternalStoragePermission() {
        var changed = false;
        var externalPermission = "/manifest/uses-permission[@android:name='android.permission.WRITE_EXTERNAL_STORAGE']";
        var externalNode = SelectNodes(externalPermission, nameSpaceManager);
        if (externalNode == null || externalNode.Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.WRITE_EXTERNAL_STORAGE"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddAccessFineLocationPermission() {
        var changed = false;
        var locationPermission = "/manifest/uses-permission[@android:name='android.permission.ACCESS_FINE_LOCATION']";
        var locationNode = SelectNodes(locationPermission, nameSpaceManager);
        if (locationNode == null || locationNode.Count == 0) {
            var elem = CreateElement("uses-permission");
            elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.ACCESS_FINE_LOCATION"));
            manifestElement.AppendChild(elem);
            changed = true;
        }
        return changed;
    }

    internal bool AddAuthCallbacksIntentFilter(string[] authCallbackUrls) {
        var changed = false;
        XmlElement authActivityNode;
        if (authCallbackUrls.Length > 0) {
            var authActivity = "/manifest/application/activity[@android:name='com.onevcat.uniwebview.UniWebViewAuthenticationActivity']";
            var list = SelectNodes(authActivity, nameSpaceManager);
            if (list == null || list.Count == 0) {
                var created = CreateElement("activity");
                created.SetAttribute("name", AndroidXmlNamespace, "com.onevcat.uniwebview.UniWebViewAuthenticationActivity");
                created.SetAttribute("exported", AndroidXmlNamespace, "true");
                created.SetAttribute("launchMode", AndroidXmlNamespace, "singleTask");
                created.SetAttribute("configChanges", AndroidXmlNamespace, "orientation|screenSize|keyboardHidden");
                authActivityNode = created;
            } else {
                authActivityNode = list[0] as XmlElement;
            }
        } else {
            return false;
        }

        foreach (var url in authCallbackUrls) {
            var intentFilter = CreateIntentFilter(url);
            if (intentFilter != null) {
                authActivityNode?.AppendChild(intentFilter);
                changed = true;
            }
        }

        if (authActivityNode != null) {
            applicationElement.AppendChild(authActivityNode);
        }
        return changed;
    }

    private XmlElement CreateIntentFilter(string url) {
        
        var uri = new Uri(url);
        var scheme = uri.Scheme;
        if (string.IsNullOrEmpty(scheme)) {
            Debug.LogError("<UniWebView> Auth callback url contains an empty scheme. Please check the url: " + url);
            return null;
        }

        var filter = CreateElement("intent-filter");
        
        var action = CreateElement("action");
        action.SetAttribute("name", AndroidXmlNamespace, "android.intent.action.VIEW");
        filter.AppendChild(action);
        
        var defaultCategory = CreateElement("category");
        defaultCategory.SetAttribute("name", AndroidXmlNamespace, "android.intent.category.DEFAULT");
        filter.AppendChild(defaultCategory);
        
        var browseCategory = CreateElement("category");
        browseCategory.SetAttribute("name", AndroidXmlNamespace, "android.intent.category.BROWSABLE");
        filter.AppendChild(browseCategory);
        
        var data = CreateElement("data");
        data.SetAttribute("scheme", AndroidXmlNamespace, scheme);
        if (!String.IsNullOrEmpty(uri.Host)) {
            data.SetAttribute("host", AndroidXmlNamespace, uri.Host);
        }
        if (uri.Port != -1) {
            data.SetAttribute("port", AndroidXmlNamespace, uri.Port.ToString());
        }
        if (!string.IsNullOrEmpty(uri.PathAndQuery) && uri.PathAndQuery != "/") {
            data.SetAttribute("path", AndroidXmlNamespace, uri.PathAndQuery);
        }
        
        filter.AppendChild(data);
        return filter;
    }
}