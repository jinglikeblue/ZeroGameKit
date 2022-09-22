using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.DeviceSimulator;

namespace UnityEditor.DeviceSimulation
{
    internal class PluginController : IDisposable
    {
        private readonly List<DeviceSimulatorPlugin> plugins = new List<DeviceSimulatorPlugin>();

        public PluginController(SimulatorState serializedState, DeviceSimulator deviceSimulator)
        {
            var nonInitializablePlugins = TypeCache.GetTypesWithAttribute<NotInitializePluginAttribute>();
            foreach (var type in TypeCache.GetTypesDerivedFrom<DeviceSimulatorPlugin>().Where(type => !type.IsAbstract && !nonInitializablePlugins.Contains(type)))
            {
                try
                {
                    var plugin = (DeviceSimulatorPlugin)Activator.CreateInstance(type);
                    plugin.deviceSimulator = deviceSimulator;
                    if (serializedState.plugins.TryGetValue(plugin.pluginId, out var serializedPlugin))
                        JsonUtility.FromJsonOverwrite(serializedPlugin, plugin);
                    try
                    {
                        plugin.OnCreate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    try
                    {
                        plugin.resolvedTitle = plugin.title;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    plugins.Add(plugin);
                }
                catch (MissingMethodException)
                {
                    Debug.LogError($"Failed instantiating Device Simulator plug-in {type.Name}. It does not have a public default constructor.");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            plugins.AddRange(RetrieveLegacyExtensions(serializedState, deviceSimulator));
            plugins.Sort(ComparePluginOrder);
        }

        public void StoreSerializationStates(ref SimulatorState states)
        {
            foreach (var plugin in plugins)
            {
                var serializedPlugin = JsonUtility.ToJson(plugin);
                states.plugins.Add(plugin.pluginId, serializedPlugin);
            }
        }

        private IEnumerable<DeviceSimulatorPlugin> RetrieveLegacyExtensions(SimulatorState serializedState, DeviceSimulator deviceSimulator)
        {
            var extensions = new SimulatorExtensions().Extensions;
            var extensionPlugins = new List<DeviceSimulatorPlugin>();

            foreach (var extension in extensions)
            {
                var plugin = new DeviceSimulatorExtensionWrapperPlugin(extension);
                plugin.deviceSimulator = deviceSimulator;

                try
                {
                    if (serializedState.plugins.TryGetValue(plugin.pluginId, out var serializedPlugin))
                        JsonUtility.FromJsonOverwrite(serializedPlugin, extension);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed deserializing Device Simulator extension {plugin.pluginId}. {e.GetType()}: {e.Message}");
                    Debug.LogException(e);
                }

                extensionPlugins.Add(plugin);
            }

            return extensionPlugins;
        }

        private static int ComparePluginOrder(DeviceSimulatorPlugin ext1, DeviceSimulatorPlugin ext2)
        {
            return string.CompareOrdinal(ext1.resolvedTitle, ext2.resolvedTitle);
        }

        public List<(VisualElement ui, string title, string serializationKey)> CreateUI()
        {
            var pluginUI = new List<(VisualElement ui, string title, string serializationKey)>();
            foreach (var plugin in plugins)
            {
                try
                {
                    var ui = plugin.OnCreateUI();
                    if (ui == null)
                        continue;

                    if (!string.IsNullOrEmpty(plugin.resolvedTitle))
                        pluginUI.Add((ui, plugin.title, plugin.pluginId));
                    else
                        Debug.LogError($"Device Simulator plug-in {plugin.pluginId} returned an empty title. It will not be added to the Control Panel.");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return pluginUI;
        }

        public void Dispose()
        {
            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.OnDestroy();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
