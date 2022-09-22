using System;
using Unity.DeviceSimulator;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.DeviceSimulation
{
    [NotInitializePluginAttribute]
    internal class DeviceSimulatorExtensionWrapperPlugin : DeviceSimulatorPlugin, ISerializationCallbackReceiver
    {
        private IDeviceSimulatorExtension m_Extension;
        [SerializeField]
        private string m_ExtensionState;

        internal DeviceSimulatorExtensionWrapperPlugin(IDeviceSimulatorExtension extension)
        {
            m_Extension = extension;
            try
            {
                resolvedTitle = extension.extensionTitle;
            }
            catch (Exception e)
            {
                LogErrorAndException($"Failed reading title of Device Simulator extension {extension.GetType().Name}.", e);
            }
        }

        internal override string pluginId => m_Extension.GetType().ToString();
        public override string title => resolvedTitle;

        public override VisualElement OnCreateUI()
        {
            try
            {
                VisualElement root = new VisualElement();
                m_Extension.OnExtendDeviceSimulator(root);
                return root;
            }
            catch (Exception e)
            {
                LogErrorAndException($"Failed creating UI for Device Simulator extension {title}.", e);
                return null;
            }
        }

        public void OnBeforeSerialize()
        {
            try
            {
                m_ExtensionState = JsonUtility.ToJson(m_Extension);
            }
            catch (Exception e)
            {
                LogErrorAndException($"Failed serializing Device Simulator extension {title}.", e);
            }
        }

        public void OnAfterDeserialize()
        {
            try
            {
                JsonUtility.FromJsonOverwrite(m_ExtensionState, m_Extension);
            }
            catch (Exception e)
            {
                LogErrorAndException($"Failed deserializing Device Simulator extension {title}.", e);
            }
        }

        private void LogErrorAndException(string message, Exception exception)
        {
            Debug.LogError($"{message} {exception.GetType().Name}: {exception.Message}");
            Debug.LogException(exception);
        }
    }
}
