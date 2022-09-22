using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.DeviceSimulation;

namespace Unity.DeviceSimulator.Editor.Tests.Utilities
{
    internal class DeviceInfoLibrary
    {
        // No notch
        public static DeviceInfo GetIphone8()
        {
            var deviceName = @"Apple iPhone 8";
            return LoadDevice(deviceName);
        }

        // Notch
        public static DeviceInfo GetGalaxy10e()
        {
            var deviceName = @"Samsung Galaxy S10e";
            return LoadDevice(deviceName);
        }

        // Notch
        public static DeviceInfo GetMotoG7Power()
        {
            var deviceName = @"Motorola Moto G7 Power";
            return LoadDevice(deviceName);
        }

        // Notch
        public static DeviceInfo GetIphoneXMax()
        {
            var deviceName = @"Apple iPhone XS Max";
            return LoadDevice(deviceName);
        }

        public static DeviceInfo LoadDevice(string name)
        {
            return DeviceLoader.LoadDevices().First(device => device.deviceInfo.friendlyName == name).deviceInfo;
        }

        public static DeviceInfo GetDeviceWithSupportedOrientations(ScreenOrientation[] orientations, int screenWidth = 500, int screenHeight = 1000, Rect portraitSafeArea = default, float screenDpi = 200)
        {
            if (portraitSafeArea == default)
                portraitSafeArea = new Rect(0, 0, 500, 1000);

            if (orientations.Length > 4)
                throw new ArgumentException("There are 4 possible screen orientations");

            var screen = new ScreenData()
            {
                dpi = screenDpi,
                width = screenWidth,
                height = screenHeight,
                orientations = new OrientationData[orientations.Length]
            };

            for (int i = 0; i < orientations.Length; i++)
            {
                var orientationData = new OrientationData();
                orientationData.orientation = orientations[i];

                switch (orientations[i])
                {
                    case ScreenOrientation.Portrait:
                        orientationData.safeArea = portraitSafeArea;
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        orientationData.safeArea = new Rect(screenWidth - portraitSafeArea.x - portraitSafeArea.width, screenHeight - portraitSafeArea.y - portraitSafeArea.height, portraitSafeArea.width, portraitSafeArea.height);
                        break;
                    case ScreenOrientation.LandscapeLeft:
                        orientationData.safeArea = new Rect(screenWidth - portraitSafeArea.y - portraitSafeArea.height, portraitSafeArea.x, portraitSafeArea.height, portraitSafeArea.width);
                        break;
                    case ScreenOrientation.LandscapeRight:
                        orientationData.safeArea = new Rect(portraitSafeArea.y, screenWidth - portraitSafeArea.x - portraitSafeArea.width, portraitSafeArea.height, portraitSafeArea.width);
                        break;
                }

                orientationData.cutouts = null;
                screen.orientations[i] = orientationData;
            }

            var device = new DeviceInfo()
            {
                screens = new[] {screen}
            };
            return device;
        }

        public static OrientationData[] Orientations =
        {
            new OrientationData() {safeArea = new Rect(100, 100, 100, 100)},
            new OrientationData() {safeArea = new Rect(200, 200, 200, 200)},
            new OrientationData() {safeArea = new Rect(300, 300, 300, 300)},
            new OrientationData() {safeArea = new Rect(400, 400, 400, 400)}
        };
    }
}
