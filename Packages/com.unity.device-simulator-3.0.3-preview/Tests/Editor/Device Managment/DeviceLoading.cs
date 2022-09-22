using System.Linq;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace DeviceManagment
{
    internal class DeviceLoading
    {
        [Test]
        public void DeviceOverlayLoad()
        {
            var devices = DeviceLoader.LoadDevices();
            var minimalDevice = devices.First(device => device.deviceInfo.friendlyName == "MinimalTestDevice1");

            var screenOverlay0 = DeviceLoader.LoadOverlay(minimalDevice, 0);
            var screenOverlay1 = DeviceLoader.LoadOverlay(minimalDevice, 1);

            Assert.NotNull(screenOverlay0);
            Assert.NotNull(screenOverlay1);
        }

        [Test]
        public void DeviceOverlayTextureValidation()
        {
            var devices = DeviceLoader.LoadDevices();

            foreach (DeviceInfoAsset deviceAsset in devices)
            {
                var device = deviceAsset.deviceInfo;
                var filePath = Path.Combine(deviceAsset.directory, device.screens[0].presentation.overlayPath);

                if (filePath.Contains("/SimulatorResources/"))
                {
                    var textureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
                    if (textureImporter != null)
                    {
                        Assert.AreEqual(TextureImporterType.GUI, textureImporter.textureType, deviceAsset.deviceInfo.ToString()+" overlay texture texture type mismatch");
                        Assert.AreEqual(TextureImporterNPOTScale.None, textureImporter.npotScale, deviceAsset.deviceInfo.ToString()+" overlay texture npot mismatch");
                        Assert.AreEqual(TextureImporterCompression.Uncompressed, textureImporter.textureCompression, deviceAsset.deviceInfo.ToString()+" overlay texture compression mode mismatch");
                        Assert.AreEqual(FilterMode.Trilinear, textureImporter.filterMode, deviceAsset.deviceInfo.ToString()+" overlay texture filter mode mismatch");
                        Assert.AreEqual(8192, textureImporter.maxTextureSize, deviceAsset.deviceInfo.ToString()+" overlay texture size mismatch");
                        Assert.IsTrue(textureImporter.isReadable, deviceAsset.deviceInfo.ToString()+": overlay texture needs to be readable");
                        Assert.IsFalse(textureImporter.mipmapEnabled, deviceAsset.deviceInfo.ToString()+": overlay texture mipmap needs to be disabled");
                    }
                }
            }
        }
    }
}
