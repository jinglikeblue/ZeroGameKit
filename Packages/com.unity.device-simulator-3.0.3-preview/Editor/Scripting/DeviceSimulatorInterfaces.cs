using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.DeviceSimulator
{
    /// <summary>
    /// Interface which provides the functionality to extend the device simulator UI.
    /// Simulator Extensions are obsolete in Unity 2021.1 and up. Please use Device Simulator Plugin instead.
    /// </summary>
    public interface IDeviceSimulatorExtension
    {
        /// <summary>
        /// Title which is shown as the title of the extended UI.
        /// </summary>
        /// <value>Title property returns the title of the extended UI.</value>
        string extensionTitle { get; }

        /// <summary>
        /// Called by Device Simulator during initialisation. Allows adding custom widgets to the simulator window.
        /// </summary>
        /// <param name="visualElement">Root [VisualElement](https://docs.unity3d.com/ScriptReference/UIElements.VisualElement.html)
        /// to which extension UI should be added.</param>
        void OnExtendDeviceSimulator(VisualElement visualElement);
    }

    internal class SimulatorExtensions
    {
        private List<IDeviceSimulatorExtension> m_Extensions = new List<IDeviceSimulatorExtension>();
        public List<IDeviceSimulatorExtension> Extensions => m_Extensions;

        public  SimulatorExtensions()
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom<IDeviceSimulatorExtension>())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                try
                {
                    AddToList(type, ref m_Extensions);
                }
                catch
                {
                    Debug.LogError($"Failed instantiating Device Simulator legacy extension {type.Name}.");
                }
            }

            m_Extensions.Sort(CompareExtensionOrder);
        }

        static void AddToList<T>(Type type, ref List<T> list) where T : class
        {
            T obj = Activator.CreateInstance(type) as T;
            list.Add(obj);
        }

        internal static int CompareExtensionOrder(IDeviceSimulatorExtension ext1, IDeviceSimulatorExtension ext2)
        {
            try
            {
                return string.Compare(ext1.extensionTitle, ext2.extensionTitle);
            }
            catch
            {
                return 0;
            }
        }
    }
}
