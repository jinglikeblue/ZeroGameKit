using System.Linq;
using System.Text;
using System.IO;

public class UniWebViewGradlePropertyPatcher {
    
    private readonly string filePath;
    private readonly UniWebViewEditorSettings settings;

    // Construct a patcher with file path.
    public UniWebViewGradlePropertyPatcher(string filePath, UniWebViewEditorSettings settings)
    {
        this.filePath = filePath;
        this.settings = settings;
    }
    
    public void Patch()
    {
        var result = UpdatedString();
        File.WriteAllText(filePath, result);
    }

    public string UpdatedString()
    {
        var lines = File.ReadAllLines(filePath);

        var hasAndroidXProperty = lines.Any(text => text.Contains("android.useAndroidX"));
        var hasJetifierProperty = lines.Any(text => text.Contains("android.enableJetifier"));

        var builder = new StringBuilder();

        foreach(var each in lines) {
            builder.AppendLine(each);
        }
        
        if (!hasAndroidXProperty) {
            builder.AppendLine("android.useAndroidX=true");
        }

        if (!hasJetifierProperty && settings.enableJetifier) {
            builder.AppendLine("android.enableJetifier=true");
        }

        // AppendLine will add a new line at the end of the string, so we need to trim it to keep the file clean.
        return builder.ToString().Trim();
    }
}