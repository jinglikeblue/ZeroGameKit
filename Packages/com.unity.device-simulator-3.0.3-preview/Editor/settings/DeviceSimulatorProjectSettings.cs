using UnityEngine;

namespace UnityEditor.DeviceSimulation
{
    internal class DeviceSimulatorProjectSettings : ScriptableObject
    {
        [SerializeField] public bool SystemInfoSimulation = true;
        [SerializeField] public bool ApplicationSimulation = true;
        [SerializeField] public bool SystemInfoDefaultAssembly;
        [SerializeField] public string[] SystemInfoAssemblies;

        public DeviceSimulatorProjectSettings()
        {
            SystemInfoSimulation = true;
            ApplicationSimulation = true;
            SystemInfoDefaultAssembly = true;
            SystemInfoAssemblies = new string[0];
        }
    }
}
