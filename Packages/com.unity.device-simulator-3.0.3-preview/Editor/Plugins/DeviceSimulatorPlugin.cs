using UnityEngine.UIElements;

namespace UnityEditor.DeviceSimulation
{
    /// <summary>
    /// Extend this class to create a Device Simulator plug-in.
    /// A plug-in lets you control custom simulation parameters from a UI inside the Control Panel of a Device Simulator window. Unity creates, destroys, and serializes a plug-in together with the Device Simulator window the plug-in is active in.
    /// </summary>
    public abstract class DeviceSimulatorPlugin
    {
        internal string resolvedTitle;
        internal virtual string pluginId => this.GetType().ToString();
        /// <summary>Device Simulator in which this plug-in is instantiated.</summary>
        /// <value>Device Simulator</value>
        public DeviceSimulator deviceSimulator { get; internal set; }
        /// <summary>Title of plug-in that is used in Control Panel.</summary>
        /// <value>Title for the plug-in UI.</value>
        public abstract string title { get; }

        /// <summary>
        /// Method used to setup plug-in when Device Simulator is opened.
        /// </summary>
        public virtual void OnCreate()
        {
        }

        /// <summary>
        /// Method used to clean up plug-in when Device Simulator is destroyed.
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// Method to build UI for Control Panel inside Device Simulator window.
        /// </summary>
        /// <returns>VisualElement containing UI for use in Control Panel.</returns>
        public virtual VisualElement OnCreateUI()
        {
            return null;
        }
    }
}
