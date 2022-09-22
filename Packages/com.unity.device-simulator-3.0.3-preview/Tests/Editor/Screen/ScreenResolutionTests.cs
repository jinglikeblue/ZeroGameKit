using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.ScreenFunctionality
{
    internal class ScreenResolutionTests
    {
        internal ScreenSimulation m_Simulation;

        [TearDown]
        public void TearDown()
        {
            m_Simulation?.Dispose();
        }

        [Test, TestCaseSource("SetResolutionCases")]
        public void SetResolution(DeviceInfo device, ScreenOrientation initOrientation, int changedWidth, int changedHeight, Rect changedSafeArea, Rect[] changedCutouts)
        {
            var eventCounter = 0;
            var eventWidth = 0;
            var eventHeight = 0;

            m_Simulation = new ScreenSimulation(device, new SimulationPlayerSettings());
            Screen.orientation = initOrientation;

            m_Simulation.OnResolutionChanged += (width, height) =>
            {
                eventCounter++;
                eventWidth = width;
                eventHeight = height;
            };

            Screen.SetResolution(changedWidth, changedHeight, true);

            Assert.AreEqual(1, eventCounter);
            Assert.AreEqual(changedWidth, eventWidth);
            Assert.AreEqual(changedWidth, Screen.currentResolution.width);
            Assert.AreEqual(changedHeight, eventHeight);
            Assert.AreEqual(changedHeight, Screen.currentResolution.height);
            Assert.AreEqual(changedSafeArea, Screen.safeArea);

            Assert.AreEqual(changedCutouts.Length, Screen.cutouts.Length);
            for (var i = 0; i < changedCutouts.Length; i++)
            {
                Assert.AreEqual(changedCutouts[i], Screen.cutouts[i]);
            }
        }

        static object[] SetResolutionCases =
        {
            new object[] { DeviceInfoLibrary.GetIphone8(), ScreenOrientation.Portrait, 500, 500, new Rect(0, 0, 500, 500), new Rect[] {}},
            new object[] { DeviceInfoLibrary.GetIphone8(), ScreenOrientation.PortraitUpsideDown, 256, 1024, new Rect(0, 0, 256, 1024), new Rect[] {}},
            // BUG https://github.com/Unity-Technologies/com.unity.device-simulator/issues/131
//            new object[] { DeviceInfoLibrary.GetIphoneXMax(), ScreenOrientation.LandscapeLeft, 3000, 3000, new Rect(147, 152, 2705, 2848), new[] {new Rect(0, 743, 100, 1512)}},
//            new object[] { DeviceInfoLibrary.GetIphoneXMax(), ScreenOrientation.LandscapeRight, 2048, 512, new Rect(101, 26, 1847, 486), new[] {new Rect(1979, 126, 68, 258)}},
            new object[] { DeviceInfoLibrary.GetGalaxy10e(), ScreenOrientation.PortraitUpsideDown, 3000, 3000, new Rect(0, 153, 3000, 2847), new[] {new Rect(3, 0, 411, 153)}},
            new object[] { DeviceInfoLibrary.GetGalaxy10e(), ScreenOrientation.LandscapeRight, 256, 1024, new Rect(0, 0, 243, 1024), new[] {new Rect(243, 0, 13, 141)}},
        };

        [Test]
        [TestCase(ScreenOrientation.Portrait, ScreenOrientation.LandscapeLeft, 500, 1000)]
        [TestCase(ScreenOrientation.LandscapeLeft, ScreenOrientation.PortraitUpsideDown, 500, 1000)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.LandscapeRight, 500, 1000)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.Portrait, 500, 1000)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.Portrait, 500, 1000)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.LandscapeLeft, 500, 1000)]
        [TestCase(ScreenOrientation.Portrait, ScreenOrientation.LandscapeLeft, 100, 10)]
        [TestCase(ScreenOrientation.LandscapeLeft, ScreenOrientation.PortraitUpsideDown, 100, 10)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.LandscapeRight, 100, 10)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.Portrait, 100, 10)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.Portrait, 100, 10)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.LandscapeLeft, 100, 10)]
        public void ScreenResolutionChangesCorrectlyWhenChangingOrientation(ScreenOrientation initOrientation, ScreenOrientation newOrientation, int screenWidth, int screenHeight)
        {
            var portraitResolution = new Vector2(screenWidth, screenHeight);
            var landscapeResolution = new Vector2(screenHeight, screenWidth);

            var testDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(ScreenTestUtilities.ExplicitOrientations, screenWidth, screenHeight);
            m_Simulation = new ScreenSimulation(testDevice, new SimulationPlayerSettings());
            var width = Screen.currentResolution.width;
            var height = Screen.currentResolution.height;
            m_Simulation.OnResolutionChanged += (w, h) =>
            {
                width = w;
                height = h;
            };

            Screen.orientation = initOrientation;
            Assert.AreEqual(initOrientation.IsLandscape() ? landscapeResolution : portraitResolution, new Vector2(width, height));

            Screen.orientation = newOrientation;
            Assert.AreEqual(newOrientation.IsLandscape() ? landscapeResolution : portraitResolution, new Vector2(width, height));
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait, ScreenOrientation.LandscapeLeft, 32, 16)]
        [TestCase(ScreenOrientation.LandscapeLeft, ScreenOrientation.PortraitUpsideDown, 32, 16)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.LandscapeRight, 32, 16)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.Portrait, 32, 16)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.Portrait, 32, 16)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.LandscapeLeft, 32, 16)]
        [TestCase(ScreenOrientation.Portrait, ScreenOrientation.LandscapeLeft, 500, 1000)]
        [TestCase(ScreenOrientation.LandscapeLeft, ScreenOrientation.PortraitUpsideDown, 500, 1000)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.LandscapeRight, 500, 1000)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.Portrait, 500, 1000)]
        [TestCase(ScreenOrientation.PortraitUpsideDown, ScreenOrientation.Portrait, 500, 1000)]
        [TestCase(ScreenOrientation.LandscapeRight, ScreenOrientation.LandscapeLeft, 500, 1000)]
        public void ScreenResolutionChangesCorrectlyWhenChangingResolutionAndOrientation(ScreenOrientation initOrientation, ScreenOrientation newOrientation, int newWidth, int newHeight)
        {
            var width = 0;
            var height = 0;
            var initResolution = new Vector2(newWidth, newHeight);
            var flippedResolution = new Vector2(newHeight, newWidth);
            var isFlipped = initOrientation.IsLandscape() ^ newOrientation.IsLandscape();

            var testDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(ScreenTestUtilities.ExplicitOrientations);
            m_Simulation = new ScreenSimulation(testDevice, new SimulationPlayerSettings());
            m_Simulation.OnResolutionChanged += (w, h) =>
            {
                width = w;
                height = h;
            };

            Screen.orientation = initOrientation;
            Screen.SetResolution(newWidth, newHeight, true);
            Assert.AreEqual(initResolution, new Vector2(width, height));

            Screen.orientation = newOrientation;
            Assert.AreEqual(isFlipped ? flippedResolution : initResolution, new Vector2(width, height));
        }
    }
}
