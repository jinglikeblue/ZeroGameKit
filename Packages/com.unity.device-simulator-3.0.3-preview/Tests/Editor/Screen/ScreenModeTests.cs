using System.Linq;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.ScreenFunctionality
{
    internal class ScreenModeTests
    {
        internal DeviceInfo m_TestDevice;
        internal ScreenSimulation m_Simulation;

        [OneTimeSetUp]
        public void SetUp()
        {
            m_TestDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(new[]
            {
                ScreenOrientation.Portrait,
                ScreenOrientation.LandscapeLeft,
                ScreenOrientation.LandscapeRight,
                ScreenOrientation.PortraitUpsideDown
            });
            m_TestDevice.systemInfo = new SystemInfoData();
        }

        [TearDown]
        public void TearDown()
        {
            m_Simulation?.Dispose();
        }

        [Test, TestCaseSource("AndroidFullScreenCases")]
        public void AndroidFullScreen(DeviceInfo device, int targetDpi, ScreenOrientation initOrientation, int windowedWidth, int windowedHeight, Vector4 windowedInsets, Rect safeArea, Rect[] cutouts)
        {
            var fullScreenEventCounter = 0;
            var insetEventCounter = 0;
            var resolutionEventCounter = 0;
            var safeAreaEventCounter = 0;

            var insetFromEvent = Vector4.zero;
            var safeAreaFromEvent = Rect.zero;

            var playerSettings = new SimulationPlayerSettings();

            if (targetDpi > 0)
            {
                playerSettings.resolutionScalingMode = ResolutionScalingMode.FixedDpi;
                playerSettings.targetDpi = targetDpi;
            }

            m_Simulation = new ScreenSimulation(device, playerSettings);
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[initOrientation];

            m_Simulation.OnResolutionChanged += (i, i1) => resolutionEventCounter++;
            m_Simulation.OnFullScreenChanged += b => fullScreenEventCounter++;
            m_Simulation.OnInsetsChanged += inset =>
            {
                insetEventCounter++;
                insetFromEvent = inset;
            };
            m_Simulation.OnScreenSpaceSafeAreaChanged += sa =>
            {
                safeAreaEventCounter++;
                safeAreaFromEvent = sa;
            };

            var fullScreenSafeArea = Screen.safeArea;
            var fullScreenResolution = Screen.currentResolution;

            Assert.IsTrue(Screen.fullScreen);

            m_Simulation.fullScreen = false;
            Assert.IsFalse(Screen.fullScreen);

            Assert.AreEqual(1, fullScreenEventCounter);
            Assert.AreEqual(1, insetEventCounter);
            Assert.AreEqual(1, resolutionEventCounter);
            Assert.AreEqual(1, safeAreaEventCounter);
            fullScreenEventCounter = 0;
            insetEventCounter = 0;
            resolutionEventCounter = 0;
            safeAreaEventCounter = 0;

            Assert.AreEqual(windowedWidth, Screen.currentResolution.width);
            Assert.AreEqual(windowedHeight, Screen.currentResolution.height);

            Assert.AreEqual(windowedInsets, insetFromEvent);
            Assert.AreEqual(windowedInsets, m_Simulation.Insets);

            Assert.AreEqual(safeArea, Screen.safeArea);

            Assert.AreEqual(cutouts.Length, Screen.cutouts.Length);
            Assert.AreEqual(cutouts.FirstOrDefault(), Screen.cutouts.FirstOrDefault());

            m_Simulation.fullScreen = true;
            Assert.IsTrue(Screen.fullScreen);

            Assert.AreEqual(1, fullScreenEventCounter);
            Assert.AreEqual(1, insetEventCounter);
            Assert.AreEqual(1, resolutionEventCounter);
            Assert.AreEqual(1, safeAreaEventCounter);

            Assert.AreEqual(fullScreenSafeArea, Screen.safeArea);
            Assert.AreEqual(fullScreenResolution, Screen.currentResolution);
        }

        static object[] AndroidFullScreenCases =
        {
            new object[] {DeviceInfoLibrary.GetMotoG7Power(), 0, ScreenOrientation.Portrait, 720, 1424, new Vector4(0, 0, 0, 96), new Rect(0, 0, 720, 1371), new Rect[] {new Rect(230, 1371, 260, 53)}},
            new object[] {DeviceInfoLibrary.GetMotoG7Power(), 0, ScreenOrientation.LandscapeRight, 1424, 720, new Vector4(0, 0, 0, 96), new Rect(0, 0, 1371, 720), new Rect[] {new Rect(1371, 230, 53, 260)}},
            new object[] {DeviceInfoLibrary.GetGalaxy10e(), 0, ScreenOrientation.LandscapeLeft, 2136, 1080, new Vector4(0, 0, 0, 144), new Rect(116, 0, 2020, 1080), new Rect[] {new Rect(0, 931, 116, 149)}},
            new object[] {DeviceInfoLibrary.GetGalaxy10e(), 0, ScreenOrientation.PortraitUpsideDown, 1080, 2020, new Vector4(0, 260, 0, 0), new Rect(0, 0, 1080, 2020), new Rect[] {}},
            new object[] {DeviceInfoLibrary.GetGalaxy10e(), 300, ScreenOrientation.Portrait, 675, 1335, new Vector4(0, 0, 0, 144), new Rect(0, 0, 675, 1262), new Rect[] {new Rect(582, 1262, 93, 72)}}
        };

        [Test]
        public void iOSFullScreen()
        {
            var fullScreenEventCounter = 0;
            var insetEventCounter = 0;
            var resolutionEventCounter = 0;
            var safeAreaEventCounter = 0;

            m_Simulation = new ScreenSimulation(DeviceInfoLibrary.GetIphoneXMax(), new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[ScreenOrientation.Portrait];

            m_Simulation.OnResolutionChanged += (i, i1) => resolutionEventCounter++;
            m_Simulation.OnFullScreenChanged += b => fullScreenEventCounter++;
            m_Simulation.OnInsetsChanged += vector4 => insetEventCounter++;
            m_Simulation.OnScreenSpaceSafeAreaChanged += rect => safeAreaEventCounter++;

            m_Simulation.fullScreen = true;
            Assert.IsTrue(Screen.fullScreen);

            m_Simulation.fullScreen = false;
            Assert.IsTrue(Screen.fullScreen);

            Assert.AreEqual(0, fullScreenEventCounter);
            Assert.AreEqual(0, insetEventCounter);
            Assert.AreEqual(0, resolutionEventCounter);
            Assert.AreEqual(0, safeAreaEventCounter);
        }

        [Test]
        [TestCase(FullScreenMode.FullScreenWindow)]
        [TestCase(FullScreenMode.ExclusiveFullScreen)]
        [TestCase(FullScreenMode.MaximizedWindow)]
        [TestCase(FullScreenMode.Windowed)]
        public void FullScreenModeAndroid(FullScreenMode fullScreenMode)
        {
            m_TestDevice.systemInfo.operatingSystem = "Android";

            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[ScreenOrientation.PortraitUpsideDown];


            m_Simulation.fullScreenMode = fullScreenMode;
            switch (fullScreenMode)
            {
                case FullScreenMode.Windowed:
                    Assert.IsFalse(Screen.fullScreen);
                    Assert.AreEqual(FullScreenMode.Windowed, Screen.fullScreenMode);
                    break;
                default:
                    Assert.IsTrue(Screen.fullScreen);
                    Assert.AreEqual(FullScreenMode.FullScreenWindow, Screen.fullScreenMode);
                    break;
            }
        }

        [Test]
        [TestCase(FullScreenMode.FullScreenWindow)]
        [TestCase(FullScreenMode.ExclusiveFullScreen)]
        [TestCase(FullScreenMode.MaximizedWindow)]
        [TestCase(FullScreenMode.Windowed)]
        public void FullScreenModeiOS(FullScreenMode fullScreenMode)
        {
            m_TestDevice.systemInfo.operatingSystem = "iOS";

            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[ScreenOrientation.PortraitUpsideDown];

            m_Simulation.fullScreenMode = fullScreenMode;
            Assert.IsTrue(Screen.fullScreen);
            Assert.AreEqual(FullScreenMode.FullScreenWindow, Screen.fullScreenMode);
        }
    }
}
