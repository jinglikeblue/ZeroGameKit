using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.DeviceSimulation
{
    internal class DeviceSimulatorProjectSettingsProvider : SettingsProvider
    {
        private DeviceSimulatorProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
        }

        private static string s_FailedToSaveError = $"Failed to save Device Simulator project settings file {k_SettingsPath}. Make sure the settings file is writable.";
        private static DeviceSimulatorProjectSettings s_LastSettings;
        private const string k_SettingsPath = "ProjectSettings/DeviceSimulatorSettings.asset";

        private SerializedObject SerializedSettings => new SerializedObject(LoadOrCreateSettings());

        [SettingsProvider]
        public static SettingsProvider CreateDeviceSimulatorSettingsProvider()
        {
            var provider = new DeviceSimulatorProjectSettingsProvider("Project/Device Simulator", SettingsScope.Project);

            provider.activateHandler = (searchContext, rootElement) =>
            {
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("packages/com.unity.device-simulator/Editor/SimulatorResources/UXML/ui_project_settings.uxml");
                visualTree.CloneTree(rootElement);
                rootElement.Bind(provider.SerializedSettings);

                rootElement.Q<Toggle>("system-info-toggle").RegisterCallback<ChangeEvent<bool>>(evt => SimulatorWindow.RestartAllSimulators());
                rootElement.Q<Toggle>("application-toggle").RegisterCallback<ChangeEvent<bool>>(evt => SimulatorWindow.RestartAllSimulators());
                rootElement.Q<Toggle>("default-assembly-toggle").RegisterCallback<ChangeEvent<bool>>(evt => SimulatorWindow.RestartAllSimulators());
                var assemblyList = rootElement.Q<ListView>("assembly-list");
                assemblyList.RegisterCallback<ChangeEvent<int>>(evt => SimulatorWindow.RestartAllSimulators());
                assemblyList.RegisterCallback<ChangeEvent<string>>(evt => SimulatorWindow.RestartAllSimulators());
            };

            return provider;
        }

        public static DeviceSimulatorProjectSettings LoadOrCreateSettings()
        {
            if (s_LastSettings != null)
                return s_LastSettings;

            DeviceSimulatorProjectSettings settings = ScriptableObject.CreateInstance<DeviceSimulatorProjectSettings>();;
            if (File.Exists(k_SettingsPath))
            {
                var settingsJson = File.ReadAllText(k_SettingsPath);
                JsonUtility.FromJsonOverwrite(settingsJson, settings);
            }

            s_LastSettings = settings;
            return settings;
        }

        private void SaveSettings()
        {
            if (s_LastSettings == null)
                return;

            var fileInfo = new FileInfo(k_SettingsPath);
            var settingsJson = JsonUtility.ToJson(s_LastSettings, true);
            if (fileInfo.Exists && settingsJson == File.ReadAllText(k_SettingsPath))
                return;

            if (!AssetDatabase.IsOpenForEdit(k_SettingsPath) && !AssetDatabase.MakeEditable(k_SettingsPath))
            {
                Debug.LogWarning(s_FailedToSaveError);
                return;
            }

            try
            {
                if (fileInfo.Exists && fileInfo.IsReadOnly)
                    fileInfo.IsReadOnly = false;
                File.WriteAllText(k_SettingsPath, settingsJson);
            }
            catch (Exception)
            {
                Debug.LogWarning(s_FailedToSaveError);
            }
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            SaveSettings();
        }
    }
}
