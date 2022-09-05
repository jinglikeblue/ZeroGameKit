using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;


public class UniWebViewGradlePropertyPatcher {
    public static void Patch(string filePath) {
        string[] lines = File.ReadAllLines(filePath);

        bool hasAndroidXProperty = lines.Any(text => text.Contains("android.useAndroidX"));
        bool hasJetifierProperty = lines.Any(text => text.Contains("android.enableJetifier"));

        StringBuilder builder = new StringBuilder();

        foreach(string each in lines) {
            builder.AppendLine(each);
        }

        if (!hasAndroidXProperty) {
            builder.AppendLine("android.useAndroidX=true");
        }

        if (!hasJetifierProperty && UniWebViewEditorSettings.GetOrCreateSettings().enableJetifier) {
            builder.AppendLine("android.enableJetifier=true");
        }

        File.WriteAllText(filePath, builder.ToString());
    }
}