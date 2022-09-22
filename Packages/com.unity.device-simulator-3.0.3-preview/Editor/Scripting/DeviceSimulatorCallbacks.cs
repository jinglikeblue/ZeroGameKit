using System;

namespace Unity.DeviceSimulator
{
    /// <summary>
    /// The callbacks provided by Device Simulator.
    /// </summary>
    public class DeviceSimulatorCallbacks
    {
        /// <summary>
        /// The callback which is triggered when changing device.
        /// </summary>
        /// <value>The callback users can register for device changing.</value>
        public static event Action OnDeviceChange;

        internal static void InvokeOnDeviceChange()
        {
            OnDeviceChange?.Invoke();
        }
    }
}
