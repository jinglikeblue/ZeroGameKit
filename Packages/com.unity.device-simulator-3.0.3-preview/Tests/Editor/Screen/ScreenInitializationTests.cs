using NUnit.Framework;
using UnityEditor;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.ScreenFunctionality
{
    // Tests to check that Player Settings correctly affect Simulator initialization
    internal class ScreenInitializationTests
    {
        internal DeviceInfo m_TestDevice;
        internal ScreenSimulation m_Simulation;

        private UIOrientation m_CachedOrientation;
        private bool m_CachedAutoPortrait;
        private bool m_CachedAutoPortraitReversed;
        private bool m_CachedAutoLandscapeLeft;
        private bool m_CachedAutoLandscapeRight;
        private int m_CachedScalingMode;
        private int m_CachedScaledDPI;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_CachedOrientation = PlayerSettings.defaultInterfaceOrientation;
            m_CachedAutoPortrait = PlayerSettings.allowedAutorotateToPortrait;
            m_CachedAutoPortraitReversed = PlayerSettings.allowedAutorotateToPortraitUpsideDown;
            m_CachedAutoLandscapeLeft = PlayerSettings.allowedAutorotateToLandscapeLeft;
            m_CachedAutoLandscapeRight = PlayerSettings.allowedAutorotateToLandscapeRight;

            var serializedSettings = PlayerSettings.GetSerializedObject();
            serializedSettings.Update();
            m_CachedScalingMode = serializedSettings.FindProperty("resolutionScalingMode").intValue;
            m_CachedScaledDPI = serializedSettings.FindProperty("targetPixelDensity").intValue;

            m_TestDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(ScreenTestUtilities.ExplicitOrientations);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PlayerSettings.defaultInterfaceOrientation = m_CachedOrientation;
            PlayerSettings.allowedAutorotateToPortrait = m_CachedAutoPortrait;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = m_CachedAutoPortraitReversed;
            PlayerSettings.allowedAutorotateToLandscapeLeft = m_CachedAutoLandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeRight = m_CachedAutoLandscapeRight;

            var serializedSettings = PlayerSettings.GetSerializedObject();
            serializedSettings.Update();
            serializedSettings.FindProperty("resolutionScalingMode").intValue = m_CachedScalingMode;
            serializedSettings.FindProperty("targetPixelDensity").intValue = m_CachedScaledDPI;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            m_Simulation?.Dispose();
        }

        [Test]
        // Basic case for all orientations
        [TestCase(UIOrientation.Portrait, 1920, 1080, 400, 333)]
        [TestCase(UIOrientation.PortraitUpsideDown, 1920, 1080, 400, 333)]
        [TestCase(UIOrientation.LandscapeLeft, 1920, 1080, 400, 333)]
        [TestCase(UIOrientation.LandscapeRight, 1920, 1080, 400, 333)]
        // More random cases where scaling should occur
        [TestCase(UIOrientation.Portrait, 200, 100, 222, 100)]
        [TestCase(UIOrientation.PortraitUpsideDown, 10, 10, 40, 35)]
        [TestCase(UIOrientation.LandscapeLeft, 2000, 50, 200, 30)]
        [TestCase(UIOrientation.LandscapeRight, 333, 333, 50, 50)]
        // Should not scale screenDpi < scaledDpi
        [TestCase(UIOrientation.Portrait, 1920, 1080, 400, 500)]
        [TestCase(UIOrientation.PortraitUpsideDown, 1920, 1080, 400, 500)]
        [TestCase(UIOrientation.LandscapeLeft, 1920, 1080, 400, 500)]
        [TestCase(UIOrientation.LandscapeRight, 1920, 1080, 400, 500)]
        public void ResolutionScalingInitializedCorrectly(UIOrientation orientation, int screenWidth, int screenHeight, float screenDpi, int scaledDpi)
        {
            PlayerSettings.defaultInterfaceOrientation = orientation;
            var serializedSettings = PlayerSettings.GetSerializedObject();
            serializedSettings.Update();
            serializedSettings.FindProperty("resolutionScalingMode").intValue = 1;
            serializedSettings.FindProperty("targetPixelDensity").intValue = scaledDpi;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();

            var testDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(ScreenTestUtilities.ExplicitOrientations, screenWidth, screenHeight, default, screenDpi);
            m_Simulation = new ScreenSimulation(testDevice, new SimulationPlayerSettings());

            if (screenDpi >= scaledDpi)
            {
                var scale = scaledDpi / screenDpi;
                var expectedResolution = orientation.IsLandscape()
                    ? new Vector2((int)(screenHeight * scale), (int)(screenWidth * scale))
                    : new Vector2((int)(screenWidth * scale), (int)(screenHeight * scale));

                Assert.AreEqual(expectedResolution, new Vector2(Screen.currentResolution.width, Screen.currentResolution.height));
            }
            else
            {
                var expectedResolution = orientation.IsLandscape()
                    ? new Vector2(screenHeight, screenWidth)
                    : new Vector2(screenWidth, screenHeight);

                Assert.AreEqual(expectedResolution, new Vector2(Screen.currentResolution.width, Screen.currentResolution.height));
            }
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void LaunchCorrectAutoOrientation(ScreenOrientation orientation)
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];
            Assert.AreEqual(orientation, Screen.orientation);
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void ExplicitOrientationInitializedFromPlayerSettings(ScreenOrientation orientation)
        {
            PlayerSettings.defaultInterfaceOrientation = ScreenTestUtilities.ScreenOrientationToUI[orientation];
            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];

            Assert.AreEqual(orientation, Screen.orientation);
        }

        [Test]
        [TestCase(true, true, true, true)]
        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, false, false, true)]
        public void AutoOrientationsInitializedFromPlayerSettings(bool portrait, bool portraitUpsideDown, bool landscapeLeft, bool landscapeRight)
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;

            // Setting all to true to avoid setting all to false, that would throw an exception and enable Portrait
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            PlayerSettings.allowedAutorotateToPortrait = portrait;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = portraitUpsideDown;
            PlayerSettings.allowedAutorotateToLandscapeLeft = landscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeRight = landscapeRight;

            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());

            Assert.AreEqual(portrait, Screen.autorotateToPortrait);
            Assert.AreEqual(portraitUpsideDown, Screen.autorotateToPortraitUpsideDown);
            Assert.AreEqual(landscapeLeft, Screen.autorotateToLandscapeLeft);
            Assert.AreEqual(landscapeRight, Screen.autorotateToLandscapeRight);
        }
    }
}
