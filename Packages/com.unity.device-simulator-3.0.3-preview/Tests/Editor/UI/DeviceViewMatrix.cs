using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.UI
{
    internal class DeviceViewMatrix
    {
        private DeviceView m_DeviceView;

        internal class DeviceViewMatrixCase
        {
            public DeviceInfo DeviceInfo;
            public float Rotation;            // A physical rotation of the device 0 - portrait, 90 - LandscapeRight, 180 - PortraitUpSideDown, 270 - LandscapeLeft
            public float Scale;
            public Vector2 InputPosition;
            public Vector2 ResultPosition;
        }

        public static IEnumerable<DeviceViewMatrixCase> DeviceViewMatrixCases()
        {
            yield return new DeviceViewMatrixCase
            {
                DeviceInfo = DeviceInfoLibrary.GetGalaxy10e(), Rotation = 0, Scale = 1.0f,
                InputPosition = new Vector2(200, 400),
                ResultPosition = new Vector2(150, 350)
            };
            yield return new DeviceViewMatrixCase
            {
                DeviceInfo = DeviceInfoLibrary.GetGalaxy10e(), Rotation = 90, Scale = 1.0f,
                InputPosition = new Vector2(200, 400),
                ResultPosition = new Vector2(350, 2165)
            };
            yield return new DeviceViewMatrixCase
            {
                DeviceInfo = DeviceInfoLibrary.GetGalaxy10e(), Rotation = 180, Scale = 1.0f,
                InputPosition = new Vector2(200, 400),
                ResultPosition = new Vector2(930, 1965)
            };
            yield return new DeviceViewMatrixCase
            {
                DeviceInfo = DeviceInfoLibrary.GetGalaxy10e(), Rotation = 270, Scale = 1.0f,
                InputPosition = new Vector2(200, 400),
                ResultPosition = new Vector2(730, 150)
            };
            yield return new DeviceViewMatrixCase
            {
                DeviceInfo = DeviceInfoLibrary.GetGalaxy10e(), Rotation = 0, Scale = 0.2f,
                InputPosition = new Vector2(200, 400),
                ResultPosition = new Vector2(950, 1950)
            };
        }

        [Test]
        public void DeviceViewMatrixSimplePasses([ValueSource(nameof(DeviceViewMatrixCases))] DeviceViewMatrixCase testData)
        {
            m_DeviceView = new DeviceView(Quaternion.Euler(0, 0, testData.Rotation), testData.Scale);
            var screen = testData.DeviceInfo.screens[0];
            m_DeviceView.SetDevice(screen.width, screen.height, screen.presentation.borderSize);

            var output = (Vector2)m_DeviceView.ViewToScreen.MultiplyPoint(testData.InputPosition);

            Assert.AreEqual(testData.ResultPosition.x, output.x, 0.1);
            Assert.AreEqual(testData.ResultPosition.y, output.y, 0.1);
        }
    }
}
