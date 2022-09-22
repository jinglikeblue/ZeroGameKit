using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.DeviceSimulation
{
    using DeviceSimulation_DeviceInfoAsset = DeviceSimulation.DeviceInfoAsset;

    internal static class DeviceLoader
    {
        public static DeviceInfoAsset[] LoadDevices()
        {
            var devices = new List<DeviceInfoAsset>();

            var deviceInfoGUIDs = AssetDatabase.FindAssets("t:DeviceInfoAsset");
            foreach (var guid in deviceInfoGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var deviceAsset = AssetDatabase.LoadAssetAtPath<DeviceInfoAsset>(assetPath);
                if (deviceAsset.parseErrors == null || deviceAsset.parseErrors.Length == 0)
                {
                    deviceAsset.directory = Path.GetDirectoryName(assetPath);
                    devices.Add(deviceAsset);
                }
            }

            devices.Sort((x, y) => string.CompareOrdinal(x.deviceInfo.friendlyName, y.deviceInfo.friendlyName));
            return devices.ToArray();
        }

        public static Texture LoadOverlay(DeviceInfoAsset device, int screenIndex)
        {
            var screen = device.deviceInfo.screens[screenIndex];
            var path = screen.presentation.overlayPath;

            if (string.IsNullOrEmpty(path))
                return null;

            if (device.editorResource)
            {
                var filePath = Path.Combine(device.directory, screen.presentation.overlayPath);
                var overlay = EditorGUIUtility.Load(filePath) as Texture;
                Debug.Assert(overlay != null, $"Failed to load built-in device {device.deviceInfo} overlay");
                return overlay;
            }

            path = path.Replace("\\", "/");
            if (!path.StartsWith("Assets/") && !path.StartsWith("Packages/"))
                path = Path.Combine(device.directory, path);

            // Custom textures need to be readable for us to accurately map touches in cutouts
            // but we can full back to the .device cutouts. We need the .meta file to be unlocked in VCS.
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null && !textureImporter.isReadable)
            {
                if (AssetDatabase.MakeEditable(path + ".meta"))
                {
                    textureImporter.isReadable = true;
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning(
                        "Read/Write not enabled on simulator texture\nRead/Write is required to simulate touch over the device bezel and cutouts accurately.");
                }
            }

            return AssetDatabase.LoadAssetAtPath<Texture>(path);
        }
    }
}
