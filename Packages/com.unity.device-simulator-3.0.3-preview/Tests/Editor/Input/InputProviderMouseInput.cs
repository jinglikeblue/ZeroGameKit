using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.DeviceSimulator.Editor.Tests.Utilities;

namespace Unity.DeviceSimulator.Editor.Tests.Input
{
    internal class InputProviderMouseInput
    {
        private DeviceInfo m_DeviceInfo;
        private TouchEventManipulator m_InputProvider;
        private ScreenSimulation m_ScreenSimulation;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            m_InputProvider?.Dispose();
            m_ScreenSimulation?.Dispose();

            yield return new ExitPlayMode();
        }

        private void SetUpSimulation(DeviceInfo deviceInfo)
        {
            m_DeviceInfo = deviceInfo;
            var screen = m_DeviceInfo.screens[0];

            m_InputProvider = new TouchEventManipulator(new UnityEditor.DeviceSimulation.DeviceSimulator());
            m_ScreenSimulation = new ScreenSimulation(m_DeviceInfo, new SimulationPlayerSettings());
            m_InputProvider.InitTouchInput(null, deviceInfo, m_ScreenSimulation);
        }

        internal class InputProviderTouchFromMouseCase
        {
            public DeviceInfo deviceInfo;
            public ScreenOrientation screenOrientation;
            public int resolutionWidth;
            public int resolutionHeight;
            public Vector2[] inputTouchPosition;
            public Vector2[] resultTouchPosition;
        }

        public static IEnumerable<InputProviderTouchFromMouseCase> InputProviderTouchFromMouseCases()
        {
            // Custom orientation, no resolution change
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.Portrait, resolutionWidth = 1080, resolutionHeight = 2280,
                inputTouchPosition = new[] { new Vector2(1, 1), new Vector2(1000, 1500), new Vector2(100, 100) },
                resultTouchPosition = new[] { new Vector2(1, 2279), new Vector2(1000, 780), new Vector2(100, 2180) }
            };
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.LandscapeLeft, resolutionWidth = 2280, resolutionHeight = 1080,
                inputTouchPosition = new[] { new Vector2(1, 1), new Vector2(1000, 333), new Vector2(99, 100) },
                resultTouchPosition = new[] { new Vector2(1, 1), new Vector2(333, 1000), new Vector2(100, 99) }
            };
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.PortraitUpsideDown, resolutionWidth = 1080, resolutionHeight = 2280,
                inputTouchPosition = new[] { new Vector2(1, 8), new Vector2(1000, 1503), new Vector2(100, 100) },
                resultTouchPosition = new[] { new Vector2(1079, 8), new Vector2(80, 1503), new Vector2(980, 100) }
            };
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.LandscapeRight, resolutionWidth = 2280, resolutionHeight = 1080,
                inputTouchPosition = new[] { new Vector2(1, 1), new Vector2(1000, 333), new Vector2(100, 100) },
                resultTouchPosition = new[] { new Vector2(2279, 1079), new Vector2(1947, 80), new Vector2(2180, 980) }
            };

            // Custom orientation and custom resolution
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.Portrait, resolutionWidth = 2000, resolutionHeight = 4000,
                inputTouchPosition = new[] { new Vector2(1, 1), new Vector2(1079, 2279), new Vector2(99, 699) },
                resultTouchPosition = new[] { new Vector2(1.85f, 3998.24f), new Vector2(1998.15f, 1.75f), new Vector2(183.33f, 2773.68f) }
            };
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.LandscapeLeft, resolutionWidth = 1140, resolutionHeight = 540,
                inputTouchPosition = new[] { new Vector2(1, 1), new Vector2(1000, 333), new Vector2(99, 100) },
                resultTouchPosition = new[] { new Vector2(0.5f, 0.5f), new Vector2(166.5f, 500), new Vector2(50, 49.5f) }
            };
            yield return new InputProviderTouchFromMouseCase
            {
                deviceInfo = DeviceInfoLibrary.GetGalaxy10e(),
                screenOrientation = ScreenOrientation.PortraitUpsideDown, resolutionWidth = 2280, resolutionHeight = 1080,
                inputTouchPosition = new[] { new Vector2(1, 8), new Vector2(1000, 1503), new Vector2(100, 100) },
                resultTouchPosition = new[] { new Vector2(2277.88f, 3.79f), new Vector2(168.88f, 711.94f), new Vector2(2068.88f, 47.36f) }
            };
        }

        [UnityTest]
        public IEnumerator LegacyInputTouchFromMouse(
            [ValueSource("InputProviderTouchFromMouseCases")] InputProviderTouchFromMouseCase testData)
        {
            Assert.AreEqual(3, testData.inputTouchPosition.Length);
            Assert.AreEqual(3, testData.resultTouchPosition.Length);

            void AssertPosition(Vector2 exp)
            {
                var p = UnityEngine.Input.touches[0].position;
                Assert.AreEqual(exp.x, p.x, 0.1);
                Assert.AreEqual(exp.y, p.y, 0.1);
            }

            SetUpSimulation(testData.deviceInfo);
            Screen.orientation = testData.screenOrientation;

            if (m_ScreenSimulation.Width != testData.resolutionWidth || m_ScreenSimulation.Height != testData.resolutionHeight)
            {
                Screen.SetResolution(testData.resolutionWidth, testData.resolutionHeight, FullScreenMode.FullScreenWindow);
            }

            yield return null;

            m_InputProvider.TouchFromMouse(testData.inputTouchPosition[0], MousePhase.Start);
            yield return null;

            Assert.AreEqual(1, UnityEngine.Input.touchCount);
            AssertPosition(testData.resultTouchPosition[0]);
            Assert.AreEqual(UnityEngine.TouchPhase.Began, UnityEngine.Input.touches[0].phase);

            yield return null;

            Assert.AreEqual(1, UnityEngine.Input.touchCount);
            AssertPosition(testData.resultTouchPosition[0]);
            Assert.AreEqual(UnityEngine.TouchPhase.Stationary, UnityEngine.Input.touches[0].phase);

            m_InputProvider.TouchFromMouse(testData.inputTouchPosition[1], MousePhase.Move);
            yield return null;

            Assert.AreEqual(1, UnityEngine.Input.touchCount);
            AssertPosition(testData.resultTouchPosition[1]);
            Assert.AreEqual(UnityEngine.TouchPhase.Moved, UnityEngine.Input.touches[0].phase);

            yield return null;

            Assert.AreEqual(1, UnityEngine.Input.touchCount);
            AssertPosition(testData.resultTouchPosition[1]);

            m_InputProvider.TouchFromMouse(testData.inputTouchPosition[2], MousePhase.End);
            yield return null;

            Assert.AreEqual(1, UnityEngine.Input.touchCount);
            AssertPosition(testData.resultTouchPosition[2]);
            Assert.AreEqual(UnityEngine.TouchPhase.Ended, UnityEngine.Input.touches[0].phase);

            yield return null;

            Assert.AreEqual(0, UnityEngine.Input.touchCount);
        }
    }
}
