using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.ScreenFunctionality
{
    internal class ScreenOrientationSupportTests
    {
        internal ScreenSimulation m_Simulation;

        [TearDown]
        public void TearDown()
        {
            m_Simulation.Dispose();
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void WillRotateOnlyToSupportedOrientationsWhenExplicitlySet(ScreenOrientation unsupportedOrientation)
        {
            var supportedOrientations = new List<ScreenOrientation>(ScreenTestUtilities.ExplicitOrientations);
            supportedOrientations.Remove(unsupportedOrientation);

            var testDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(supportedOrientations.ToArray());

            m_Simulation = new ScreenSimulation(testDevice, new SimulationPlayerSettings());
            foreach (var orientation in supportedOrientations)
            {
                Screen.orientation = orientation;
                Assert.AreEqual(orientation, Screen.orientation);
            }
            Screen.orientation = unsupportedOrientation;
            Assert.AreNotEqual(unsupportedOrientation, Screen.orientation);
        }

        [Test]
        [TestCase(ScreenOrientation.Portrait)]
        [TestCase(ScreenOrientation.PortraitUpsideDown)]
        [TestCase(ScreenOrientation.LandscapeLeft)]
        [TestCase(ScreenOrientation.LandscapeRight)]
        public void WillRotateOnlyToSupportedOrientationsWhenAutoRotating(ScreenOrientation unsupportedOrientation)
        {
            var supportedOrientations = new List<ScreenOrientation>(ScreenTestUtilities.ExplicitOrientations);
            supportedOrientations.Remove(unsupportedOrientation);

            var testDevice = DeviceInfoLibrary.GetDeviceWithSupportedOrientations(supportedOrientations.ToArray());

            m_Simulation = new ScreenSimulation(testDevice, new SimulationPlayerSettings());

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            foreach (var orientation in supportedOrientations)
            {
                m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[orientation];
                Assert.AreEqual(orientation, Screen.orientation);
            }
            m_Simulation.DeviceRotation = ScreenTestUtilities.OrientationRotation[unsupportedOrientation];
            Assert.AreNotEqual(unsupportedOrientation, Screen.orientation);
        }
    }
}
