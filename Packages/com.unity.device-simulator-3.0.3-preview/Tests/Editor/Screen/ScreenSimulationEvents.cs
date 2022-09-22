using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.ScreenFunctionality
{
    internal class ScreenSimulationEvents
    {
        private DeviceInfo m_TestDevice;
        private ScreenSimulation m_Simulation;

        private int m_EventCounter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
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
        public void OnOrientationChangedTest()
        {
            m_Simulation = new ScreenSimulation(m_TestDevice, new SimulationPlayerSettings());

            var autoRotate = false;
            void Reset()
            {
                m_EventCounter = 0;
                autoRotate = false;
            }

            m_Simulation.OnOrientationChanged += auto =>
            {
                m_EventCounter++;
                autoRotate = auto;
            };

            Reset();
            Screen.orientation = ScreenOrientation.AutoRotation;
            Assert.AreEqual(1, m_EventCounter);
            Assert.IsTrue(autoRotate);

            Reset();
            Screen.orientation = ScreenOrientation.Portrait;
            Assert.AreEqual(1, m_EventCounter);
            Assert.IsFalse(autoRotate);

            Reset();
            Screen.orientation = ScreenOrientation.PortraitUpsideDown;
            Assert.AreEqual(1, m_EventCounter);
            Assert.IsFalse(autoRotate);

            Reset();
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Assert.AreEqual(1, m_EventCounter);
            Assert.IsFalse(autoRotate);

            Reset();
            Screen.orientation = ScreenOrientation.LandscapeRight;
            Assert.AreEqual(1, m_EventCounter);
            Assert.IsFalse(autoRotate);

            Reset();
            Screen.orientation = ScreenOrientation.AutoRotation;
            Assert.AreEqual(1, m_EventCounter);
            Assert.IsTrue(autoRotate);
        }
    }
}
