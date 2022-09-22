using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace UnityEditor.DeviceSimulation
{
    internal class ApplicationSettingsPlugin : DeviceSimulatorPlugin
    {
        public override string title => "Application Settings";

        public override VisualElement OnCreateUI()
        {
            var root = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("packages/com.unity.device-simulator/Editor/SimulatorResources/UXML/ui_application_settings.uxml");
            visualTree.CloneTree(root);

            var systemLanguageField = root.Q<EnumField>("application-system-language");
            systemLanguageField.Init(deviceSimulator.applicationSimulation.simulatedSystemLanguage);
            systemLanguageField.RegisterValueChangedCallback((evt) => { deviceSimulator.applicationSimulation.simulatedSystemLanguage = (SystemLanguage)evt.newValue; });

            var internetReachabilityField = root.Q<EnumField>("application-internet-reachability");
            internetReachabilityField.Init(deviceSimulator.applicationSimulation.internetReachability);
            internetReachabilityField.RegisterValueChangedCallback((evt) => { deviceSimulator.applicationSimulation.simulatedInternetReachability = (NetworkReachability)evt.newValue; });

            var onLowMemoryButton = root.Q<Button>("application-low-memory");
            onLowMemoryButton.clickable = new Clickable(() => deviceSimulator.applicationSimulation.InvokeLowMemory());

            return root;
        }
    }
}
