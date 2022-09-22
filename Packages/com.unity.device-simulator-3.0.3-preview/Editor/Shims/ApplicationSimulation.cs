using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.DeviceSimulation
{
    internal class ApplicationSimulation : UnityEngine.ApplicationShimBase
    {
        public SystemLanguage simulatedSystemLanguage { get; set; }
        public NetworkReachability simulatedInternetReachability { get; set; }

        private DeviceInfo m_DeviceInfo;
        private List<string> m_ShimmedAssemblies;

        public ApplicationSimulation(SimulatorState serializedState)
        {
            simulatedSystemLanguage = serializedState.systemLanguage;
            simulatedInternetReachability = serializedState.networkReachability;
        }

        public void OnSimulationStart(DeviceInfo deviceInfo, List<string> shimmedAssemblies)
        {
            m_DeviceInfo = deviceInfo;
            m_ShimmedAssemblies = shimmedAssemblies;
        }

        public void Enable()
        {
            ShimManager.UseShim(this);
        }

        public void Disable()
        {
            ShimManager.RemoveShim(this);
        }

        public new void Dispose()
        {
            Disable();
        }

        private bool ShouldShim()
        {
            return SimulatorUtilities.ShouldShim(m_ShimmedAssemblies);
        }

        public void StoreSerializationStates(ref SimulatorState states)
        {
            states.systemLanguage = simulatedSystemLanguage;
            states.networkReachability = simulatedInternetReachability;
        }

        public override bool isEditor => !ShouldShim();
        public override RuntimePlatform platform => ShouldShim() ? (m_DeviceInfo.IsiOSDevice() ? RuntimePlatform.IPhonePlayer : RuntimePlatform.Android) : base.platform;
        public override bool isMobilePlatform => ShouldShim() ?  m_DeviceInfo.IsMobileDevice() : base.isMobilePlatform;
        public override bool isConsolePlatform => ShouldShim() ?  m_DeviceInfo.IsConsoleDevice() : base.isConsolePlatform;
        public override SystemLanguage systemLanguage => ShouldShim() ?  simulatedSystemLanguage : base.systemLanguage;
        public override NetworkReachability internetReachability => ShouldShim() ?  simulatedInternetReachability : base.internetReachability;

        public void InvokeLowMemory()
        {
            OnLowMemory();
        }
    }
}
