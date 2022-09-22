using System.Linq;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;

namespace DeviceManagment
{
    internal class DeviceInfoValidation
    {
        [Test]
        public void MinimalDevice()
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
        ""operatingSystem"": ""Android""
    }
}
            ";
            var deviceInfo = DeviceInfoImporter.ParseDeviceInfo(deviceJson, out var parseErrors, out var e, out var g);
            Assert.IsTrue(parseErrors.Length == 0);
            DeviceInfoImporter.AddOptionalFields(deviceInfo);
            Assert.IsTrue(deviceInfo.screens[0].orientations.Length == 4);
        }

        [Test]
        public void NotJson()
        {
            var deviceJson = @"{";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoFriendlyName()
        {
            var deviceJson = @"{
    ""version"": 1,
    ""screens"": [
    {
        ""width"": 1080,
        ""height"": 1920,
        ""dpi"": 450.0
    }
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoVersion()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""screens"": [
    {
        ""width"": 1080,
        ""height"": 1920,
        ""dpi"": 450.0
    }
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoSystemInfo()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {
        ""width"": 1080,
        ""height"": 1920,
        ""dpi"": 450.0
    }
    ]
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void OperatingSystemEmpty()
        {
            var deviceJson = @"{
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
        ""operatingSystem"": """"
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoScreens()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void EmptyScreens()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoWidth()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {
        ""height"": 1920,
        ""dpi"": 450.0
    }
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoHeight()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {
        ""width"": 1080,
        ""dpi"": 450.0
    }
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoDpi()
        {
            var deviceJson = @"{
    ""friendlyName"": ""Minimal Device"",
    ""version"": 1,
    ""screens"": [
    {
        ""width"": 1080,
        ""height"": 1920
    }
    ],
    ""systemInfo"": {
        ""operatingSystem"": ""Android""
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        [Test]
        public void NoOperatingSystem()
        {
            var deviceJson = @"{
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
    }
}";
            MakeSureParsingFailed(deviceJson);
        }

        private static void MakeSureParsingFailed(string deviceJson)
        {
            DeviceInfoImporter.ParseDeviceInfo(deviceJson, out var parseErrors, out var e, out var g);
            Assert.IsTrue(parseErrors.Length > 0);
            Debug.Log(parseErrors.Aggregate((s1, s2) => s1 + "\n" + s2));
        }
    }
}
