using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.ScreenFunctionality
{
    internal class ScreenOrientationTests
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
        }

        [TearDown]
        public void TearDown()
        {
            m_Simulation?.Dispose();
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void RotateWithAutorotate(ScreenOrientation orientation)
        {
            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[ScreenOrientation.PortraitUpsideDown];

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];

            Assert.AreEqual(orientation, Screen.orientation);
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void RuntimeAutoRotationOrientationEnable(ScreenOrientation orientation)
        {
            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            ScreenTestUtilities.SetScreenAutoOrientation(orientation, false);

            Assert.AreNotEqual(orientation, Screen.orientation);
            ScreenTestUtilities.SetScreenAutoOrientation(orientation, true);
            Assert.AreEqual(orientation, Screen.orientation);
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void RuntimeAutoRotationOrientationDisable(ScreenOrientation orientation)
        {
            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            Assert.AreEqual(orientation, Screen.orientation);
            ScreenTestUtilities.SetScreenAutoOrientation(orientation, false);
            Assert.AreNotEqual(orientation, Screen.orientation);
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void SetOrientationExplicitly(ScreenOrientation orientation)
        {
            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[ScreenOrientation.PortraitUpsideDown];
            Screen.orientation = orientation;
            Assert.AreEqual(orientation, Screen.orientation);
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void WillRotateOnlyToEnabledOrientationsWhenAutoRotating(ScreenOrientation disabledOrientation)
        {
            var enabledOrientations = new List<ScreenOrientation>(ScreenTestUtilities.ExplicitOrientations);
            enabledOrientations.Remove(disabledOrientation);

            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());

            Screen.orientation = ScreenOrientation.AutoRotation;
            foreach (var orientation in enabledOrientations)
            {
                ScreenTestUtilities.SetScreenAutoOrientation(orientation, true);
            }
            ScreenTestUtilities.SetScreenAutoOrientation(disabledOrientation, false);

            foreach (var orientation in enabledOrientations)
            {
                m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];
                Assert.AreEqual(orientation, Screen.orientation);
            }
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[disabledOrientation];
            Assert.AreNotEqual(disabledOrientation, Screen.orientation);
        }
    }
}
