using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;

internal class DeviceOptionalFieldAvailability
{
    private static IEnumerable GetSystemInfoDataFields()
    {
        var mandatoryFields = new[] {"operatingSystem", "graphicsDependentData"};
        var type = typeof(SystemInfoData);
        return type.GetFields().Where(field => !mandatoryFields.Contains(field.Name));
    }

    private static IEnumerable GetGraphicsSystemInfoDataFields()
    {
        var mandatoryFields = new[] {"graphicsDeviceType"};
        var type = typeof(GraphicsSystemInfoData);
        return type.GetFields().Where(field => !mandatoryFields.Contains(field.Name));
    }

    [Test]
    public void SystemInfoNoOptionalFields()
    {
        var deviceJson = @"
{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {
        ""width"": 1080,
        ""height"": 1920,
        ""dpi"": 450.0
    }
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android"",
        ""graphicsDependentData"": [
            {
                ""graphicsDeviceType"" : 16
            },
            {
                ""graphicsDeviceType"" : 21
            }
        ]
    }
}
        ";

        DeviceInfoImporter.ParseDeviceInfo(deviceJson, out var parseErrors, out var e, out var g);
        Assert.IsTrue(parseErrors.Length == 0);
        var device = ScriptableObject.CreateInstance<DeviceInfoAsset>();
        DeviceInfoImporter.FindOptionalFieldAvailability(device, e, g);
        Assert.Zero(device.availableSystemInfoFields.Count);
        Assert.AreEqual(2, device.availableGraphicsSystemInfoFields.Count);
        foreach (var graphicsDeviceFields in device.availableGraphicsSystemInfoFields.Values)
        {
            Assert.Zero(graphicsDeviceFields.Count);
        }
    }

    [Test]
    [TestCaseSource(nameof(GetSystemInfoDataFields))]
    public void SystemInfoData(FieldInfo field)
    {
        var deviceJson = @"
{{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {{
        ""width"": 1080,
        ""height"": 1920,
        ""dpi"": 450.0
    }}
    ],
    ""systemInfo"": {{
        ""operatingSystem"": ""Android"",
        ""{0}"": ""n/a""
    }}
}}
        ";
        deviceJson = string.Format(deviceJson, field.Name);
        DeviceInfoImporter.ParseDeviceInfo(deviceJson, out var parseErrors, out var e, out var g);
        Assert.IsTrue(parseErrors.Length == 0);
        var device = ScriptableObject.CreateInstance<DeviceInfoAsset>();
        DeviceInfoImporter.FindOptionalFieldAvailability(device, e, g);
        Assert.AreEqual(1, device.availableSystemInfoFields.Count);
        Assert.True(device.availableSystemInfoFields.Contains(field.Name));
    }

    [Test]
    [TestCaseSource(nameof(GetGraphicsSystemInfoDataFields))]
    public void GraphicsSystemInfoData(FieldInfo field)
    {
        var deviceJson = @"
{{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {{
        ""width"": 1080,
        ""height"": 1920,
        ""dpi"": 450.0
    }}
    ],
    ""systemInfo"": {{
        ""operatingSystem"": ""Android"",
        ""graphicsDependentData"": [
            {{
                ""graphicsDeviceType"" : 16,
                ""{0}"": ""n/a""
            }}
        ]
    }}
}}
        ";
        deviceJson = string.Format(deviceJson, field.Name);
        DeviceInfoImporter.ParseDeviceInfo(deviceJson, out var parseErrors, out var e, out var g);
        var errorMessage = parseErrors.Length == 0 ? "" : parseErrors.Aggregate((s1, s2) => s1 + "\n" + s2);
        Assert.IsTrue(parseErrors.Length == 0, errorMessage);
        var device = ScriptableObject.CreateInstance<DeviceInfoAsset>();
        DeviceInfoImporter.FindOptionalFieldAvailability(device, e, g);
        Assert.AreEqual(1, device.availableGraphicsSystemInfoFields.Count);
        Assert.True(device.availableGraphicsSystemInfoFields.Values.First().Contains(field.Name));
    }
}
