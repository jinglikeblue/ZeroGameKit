using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace UnityEditor.DeviceSimulation
{
    internal class SystemInfoSimulation : SystemInfoShimBase
    {
        // Many SystemInfo values are optional in a .device file. HashSets store which fields are available. In case an optional field is missing we return a default value for the editor.
        private readonly SystemInfoData m_SystemInfo;
        private readonly HashSet<string> m_SystemInfoFields;
        private readonly GraphicsSystemInfoData m_GraphicsSystemInfo;
        private readonly HashSet<string> m_GraphicsSystemInfoFields;
        private readonly List<string> m_ShimmedAssemblies;
        public SystemInfoSimulation(DeviceInfoAsset device, SimulationPlayerSettings playerSettings, List<string> shimmedAssemblies)
        {
            m_SystemInfo = device.deviceInfo.systemInfo;
            m_SystemInfoFields = device.availableSystemInfoFields;
            m_ShimmedAssemblies = shimmedAssemblies;
            if (device.deviceInfo.systemInfo.graphicsDependentData?.Length > 0)
            {
                if (device.deviceInfo.IsAndroidDevice())
                {
                    m_GraphicsSystemInfo = (
                        from selected in playerSettings.androidGraphicsAPIs
                        from gfxDevice in m_SystemInfo.graphicsDependentData
                        where selected == gfxDevice.graphicsDeviceType select gfxDevice).FirstOrDefault();
                }
                else if (device.deviceInfo.IsiOSDevice())
                {
                    m_GraphicsSystemInfo = (
                        from selected in playerSettings.iOSGraphicsAPIs
                        from gfxDevice in m_SystemInfo.graphicsDependentData
                        where selected == gfxDevice.graphicsDeviceType select gfxDevice).FirstOrDefault();
                }
                if (m_GraphicsSystemInfo == null)
                {
                    Debug.LogWarning("Could not pick GraphicsDeviceType, the game would fail to launch");
                }
            }
            m_GraphicsSystemInfoFields = m_GraphicsSystemInfo != null ? device.availableGraphicsSystemInfoFields[m_GraphicsSystemInfo.graphicsDeviceType] : new HashSet<string>();
        }

        public void Enable()
        {
            ShimManager.UseShim(this);
        }

        public void Disable()
        {
            ShimManager.RemoveShim(this);
        }

        public void Dispose()
        {
            Disable();
        }

        public bool ShouldShim()
        {
            return SimulatorUtilities.ShouldShim(m_ShimmedAssemblies);
        }

        public override string operatingSystem => ShouldShim() ? m_SystemInfo.operatingSystem : base.operatingSystem;
        public override OperatingSystemFamily operatingSystemFamily => m_SystemInfoFields.Contains("operatingSystemFamily") && ShouldShim() ? m_SystemInfo.operatingSystemFamily : base.operatingSystemFamily;
        public override string processorType => m_SystemInfoFields.Contains("processorType") && ShouldShim() ? m_SystemInfo.processorType : base.processorType;
        public override int processorFrequency => m_SystemInfoFields.Contains("processorFrequency") && ShouldShim() ? m_SystemInfo.processorFrequency : base.processorFrequency;
        public override int processorCount => m_SystemInfoFields.Contains("processorCount") && ShouldShim() ? m_SystemInfo.processorCount : base.processorCount;
        public override int systemMemorySize => m_SystemInfoFields.Contains("systemMemorySize") && ShouldShim() ? m_SystemInfo.systemMemorySize : base.systemMemorySize;
        public override string deviceModel => m_SystemInfoFields.Contains("deviceModel") && ShouldShim() ? m_SystemInfo.deviceModel : base.deviceModel;
        public override bool supportsAccelerometer => m_SystemInfoFields.Contains("supportsAccelerometer") && ShouldShim() ? m_SystemInfo.supportsAccelerometer : base.supportsAccelerometer;
        public override bool supportsGyroscope => m_SystemInfoFields.Contains("supportsGyroscope") && ShouldShim() ? m_SystemInfo.supportsGyroscope : base.supportsGyroscope;
        public override bool supportsLocationService => m_SystemInfoFields.Contains("supportsLocationService") && ShouldShim() ? m_SystemInfo.supportsLocationService : base.supportsLocationService;
        public override bool supportsVibration => m_SystemInfoFields.Contains("supportsVibration") && ShouldShim() ? m_SystemInfo.supportsVibration : base.supportsVibration;
        public override bool supportsAudio => m_SystemInfoFields.Contains("supportsAudio") && ShouldShim() ? m_SystemInfo.supportsAudio : base.supportsAudio;
        public override DeviceType deviceType => m_SystemInfoFields.Contains("deviceType") && ShouldShim() ? m_SystemInfo.deviceType : base.deviceType;

        public override GraphicsDeviceType graphicsDeviceType => m_GraphicsSystemInfo?.graphicsDeviceType ?? base.graphicsDeviceType;
        public override int graphicsMemorySize  =>  m_GraphicsSystemInfoFields.Contains("graphicsMemorySize") && ShouldShim() ? m_GraphicsSystemInfo.graphicsMemorySize : base.graphicsMemorySize;
        public override string graphicsDeviceName  =>  m_GraphicsSystemInfoFields.Contains("graphicsDeviceName") && ShouldShim() ? m_GraphicsSystemInfo.graphicsDeviceName : base.graphicsDeviceName;
        public override string graphicsDeviceVendor  =>  m_GraphicsSystemInfoFields.Contains("graphicsDeviceVendor") && ShouldShim() ? m_GraphicsSystemInfo.graphicsDeviceVendor : base.graphicsDeviceVendor;
        public override int graphicsDeviceID  =>  m_GraphicsSystemInfoFields.Contains("graphicsDeviceID") && ShouldShim() ? m_GraphicsSystemInfo.graphicsDeviceID : base.graphicsDeviceID;
        public override int graphicsDeviceVendorID  =>  m_GraphicsSystemInfoFields.Contains("graphicsDeviceVendorID") && ShouldShim() ? m_GraphicsSystemInfo.graphicsDeviceVendorID : base.graphicsDeviceVendorID;
        public override bool graphicsUVStartsAtTop  =>  m_GraphicsSystemInfoFields.Contains("graphicsUVStartsAtTop") && ShouldShim() ? m_GraphicsSystemInfo.graphicsUVStartsAtTop : base.graphicsUVStartsAtTop;
        public override string graphicsDeviceVersion  =>  m_GraphicsSystemInfoFields.Contains("graphicsDeviceVersion") && ShouldShim() ? m_GraphicsSystemInfo.graphicsDeviceVersion : base.graphicsDeviceVersion;
        public override int graphicsShaderLevel  =>  m_GraphicsSystemInfoFields.Contains("graphicsShaderLevel") && ShouldShim() ? m_GraphicsSystemInfo.graphicsShaderLevel : base.graphicsShaderLevel;
        public override bool graphicsMultiThreaded  =>  m_GraphicsSystemInfoFields.Contains("graphicsMultiThreaded") && ShouldShim() ? m_GraphicsSystemInfo.graphicsMultiThreaded : base.graphicsMultiThreaded;
        public override RenderingThreadingMode renderingThreadingMode  =>  m_GraphicsSystemInfoFields.Contains("renderingThreadingMode") && ShouldShim() ? m_GraphicsSystemInfo.renderingThreadingMode : base.renderingThreadingMode;
        public override bool hasHiddenSurfaceRemovalOnGPU  =>  m_GraphicsSystemInfoFields.Contains("hasHiddenSurfaceRemovalOnGPU") && ShouldShim() ? m_GraphicsSystemInfo.hasHiddenSurfaceRemovalOnGPU : base.hasHiddenSurfaceRemovalOnGPU;
        public override bool hasDynamicUniformArrayIndexingInFragmentShaders  =>  m_GraphicsSystemInfoFields.Contains("hasDynamicUniformArrayIndexingInFragmentShaders") && ShouldShim() ? m_GraphicsSystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders : base.hasDynamicUniformArrayIndexingInFragmentShaders;
        public override bool supportsShadows  =>  m_GraphicsSystemInfoFields.Contains("supportsShadows") && ShouldShim() ? m_GraphicsSystemInfo.supportsShadows : base.supportsShadows;
        public override bool supportsRawShadowDepthSampling  =>  m_GraphicsSystemInfoFields.Contains("supportsRawShadowDepthSampling") && ShouldShim() ? m_GraphicsSystemInfo.supportsRawShadowDepthSampling : base.supportsRawShadowDepthSampling;
        public override bool supportsMotionVectors  =>  m_GraphicsSystemInfoFields.Contains("supportsMotionVectors") && ShouldShim() ? m_GraphicsSystemInfo.supportsMotionVectors : base.supportsMotionVectors;
        public override bool supports3DTextures  =>  m_GraphicsSystemInfoFields.Contains("supports3DTextures") && ShouldShim() ? m_GraphicsSystemInfo.supports3DTextures : base.supports3DTextures;
        public override bool supports2DArrayTextures  =>  m_GraphicsSystemInfoFields.Contains("supports2DArrayTextures") && ShouldShim() ? m_GraphicsSystemInfo.supports2DArrayTextures : base.supports2DArrayTextures;
        public override bool supports3DRenderTextures  =>  m_GraphicsSystemInfoFields.Contains("supports3DRenderTextures") && ShouldShim() ? m_GraphicsSystemInfo.supports3DRenderTextures : base.supports3DRenderTextures;
        public override bool supportsCubemapArrayTextures  =>  m_GraphicsSystemInfoFields.Contains("supportsCubemapArrayTextures") && ShouldShim() ? m_GraphicsSystemInfo.supportsCubemapArrayTextures : base.supportsCubemapArrayTextures;
        public override CopyTextureSupport copyTextureSupport  =>  m_GraphicsSystemInfoFields.Contains("copyTextureSupport") && ShouldShim() ? m_GraphicsSystemInfo.copyTextureSupport : base.copyTextureSupport;
        public override bool supportsComputeShaders  =>  m_GraphicsSystemInfoFields.Contains("supportsComputeShaders") && ShouldShim() ? m_GraphicsSystemInfo.supportsComputeShaders : base.supportsComputeShaders;
        public override bool supportsGeometryShaders  =>  m_GraphicsSystemInfoFields.Contains("supportsGeometryShaders") && ShouldShim() ? m_GraphicsSystemInfo.supportsGeometryShaders : base.supportsGeometryShaders;
        public override bool supportsTessellationShaders  =>  m_GraphicsSystemInfoFields.Contains("supportsTessellationShaders") && ShouldShim() ? m_GraphicsSystemInfo.supportsTessellationShaders : base.supportsTessellationShaders;
        public override bool supportsInstancing  =>  m_GraphicsSystemInfoFields.Contains("supportsInstancing") && ShouldShim() ? m_GraphicsSystemInfo.supportsInstancing : base.supportsInstancing;
        public override bool supportsHardwareQuadTopology  =>  m_GraphicsSystemInfoFields.Contains("supportsHardwareQuadTopology") && ShouldShim() ? m_GraphicsSystemInfo.supportsHardwareQuadTopology : base.supportsHardwareQuadTopology;
        public override bool supports32bitsIndexBuffer  =>  m_GraphicsSystemInfoFields.Contains("supports32bitsIndexBuffer") && ShouldShim() ? m_GraphicsSystemInfo.supports32bitsIndexBuffer : base.supports32bitsIndexBuffer;
        public override bool supportsSparseTextures  =>  m_GraphicsSystemInfoFields.Contains("supportsSparseTextures") && ShouldShim() ? m_GraphicsSystemInfo.supportsSparseTextures : base.supportsSparseTextures;
        public override int supportedRenderTargetCount  =>  m_GraphicsSystemInfoFields.Contains("supportedRenderTargetCount") && ShouldShim() ? m_GraphicsSystemInfo.supportedRenderTargetCount : base.supportedRenderTargetCount;
        public override bool supportsSeparatedRenderTargetsBlend  =>  m_GraphicsSystemInfoFields.Contains("supportsSeparatedRenderTargetsBlend") && ShouldShim() ? m_GraphicsSystemInfo.supportsSeparatedRenderTargetsBlend : base.supportsSeparatedRenderTargetsBlend;
        public override int supportedRandomWriteTargetCount  =>  m_GraphicsSystemInfoFields.Contains("supportedRandomWriteTargetCount") && ShouldShim() ? m_GraphicsSystemInfo.supportedRandomWriteTargetCount : base.supportedRandomWriteTargetCount;
        public override int supportsMultisampledTextures  =>  m_GraphicsSystemInfoFields.Contains("supportsMultisampledTextures") && ShouldShim() ? m_GraphicsSystemInfo.supportsMultisampledTextures : base.supportsMultisampledTextures;
        public override bool supportsMultisampleAutoResolve  =>  m_GraphicsSystemInfoFields.Contains("supportsMultisampleAutoResolve") && ShouldShim() ? m_GraphicsSystemInfo.supportsMultisampleAutoResolve : base.supportsMultisampleAutoResolve;
        public override int supportsTextureWrapMirrorOnce  =>  m_GraphicsSystemInfoFields.Contains("supportsTextureWrapMirrorOnce") && ShouldShim() ? m_GraphicsSystemInfo.supportsTextureWrapMirrorOnce : base.supportsTextureWrapMirrorOnce;
        public override bool usesReversedZBuffer  =>  m_GraphicsSystemInfoFields.Contains("usesReversedZBuffer") && ShouldShim() ? m_GraphicsSystemInfo.usesReversedZBuffer : base.usesReversedZBuffer;
        public override NPOTSupport npotSupport  =>  m_GraphicsSystemInfoFields.Contains("npotSupport") && ShouldShim() ? m_GraphicsSystemInfo.npotSupport : base.npotSupport;
        public override int maxTextureSize  =>  m_GraphicsSystemInfoFields.Contains("maxTextureSize") && ShouldShim() ? m_GraphicsSystemInfo.maxTextureSize : base.maxTextureSize;
        public override int maxCubemapSize  =>  m_GraphicsSystemInfoFields.Contains("maxCubemapSize") && ShouldShim() ? m_GraphicsSystemInfo.maxCubemapSize : base.maxCubemapSize;
        public override int maxComputeBufferInputsVertex  =>  m_GraphicsSystemInfoFields.Contains("maxComputeBufferInputsVertex") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeBufferInputsVertex : base.maxComputeBufferInputsVertex;
        public override int maxComputeBufferInputsFragment  =>  m_GraphicsSystemInfoFields.Contains("maxComputeBufferInputsFragment") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeBufferInputsFragment : base.maxComputeBufferInputsFragment;
        public override int maxComputeBufferInputsGeometry  =>  m_GraphicsSystemInfoFields.Contains("maxComputeBufferInputsGeometry") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeBufferInputsGeometry : base.maxComputeBufferInputsGeometry;
        public override int maxComputeBufferInputsDomain  =>  m_GraphicsSystemInfoFields.Contains("maxComputeBufferInputsDomain") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeBufferInputsDomain : base.maxComputeBufferInputsDomain;
        public override int maxComputeBufferInputsHull  =>  m_GraphicsSystemInfoFields.Contains("maxComputeBufferInputsHull") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeBufferInputsHull : base.maxComputeBufferInputsHull;
        public override int maxComputeBufferInputsCompute  =>  m_GraphicsSystemInfoFields.Contains("maxComputeBufferInputsCompute") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeBufferInputsCompute : base.maxComputeBufferInputsCompute;
        public override int maxComputeWorkGroupSize  =>  m_GraphicsSystemInfoFields.Contains("maxComputeWorkGroupSize") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeWorkGroupSize : base.maxComputeWorkGroupSize;
        public override int maxComputeWorkGroupSizeX  =>  m_GraphicsSystemInfoFields.Contains("maxComputeWorkGroupSizeX") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeWorkGroupSizeX : base.maxComputeWorkGroupSizeX;
        public override int maxComputeWorkGroupSizeY  =>  m_GraphicsSystemInfoFields.Contains("maxComputeWorkGroupSizeY") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeWorkGroupSizeY : base.maxComputeWorkGroupSizeY;
        public override int maxComputeWorkGroupSizeZ  =>  m_GraphicsSystemInfoFields.Contains("maxComputeWorkGroupSizeZ") && ShouldShim() ? m_GraphicsSystemInfo.maxComputeWorkGroupSizeZ : base.maxComputeWorkGroupSizeZ;
        public override bool supportsAsyncCompute  =>  m_GraphicsSystemInfoFields.Contains("supportsAsyncCompute") && ShouldShim() ? m_GraphicsSystemInfo.supportsAsyncCompute : base.supportsAsyncCompute;
        public override bool supportsGraphicsFence  =>  m_GraphicsSystemInfoFields.Contains("supportsGraphicsFence") && ShouldShim() ? m_GraphicsSystemInfo.supportsGraphicsFence : base.supportsGraphicsFence;
        public override bool supportsAsyncGPUReadback  =>  m_GraphicsSystemInfoFields.Contains("supportsAsyncGPUReadback") && ShouldShim() ? m_GraphicsSystemInfo.supportsAsyncGPUReadback : base.supportsAsyncGPUReadback;
        public override bool supportsRayTracing  =>  m_GraphicsSystemInfoFields.Contains("supportsRayTracing") && ShouldShim() ? m_GraphicsSystemInfo.supportsRayTracing : base.supportsRayTracing;
        public override bool supportsSetConstantBuffer  =>  m_GraphicsSystemInfoFields.Contains("supportsSetConstantBuffer") && ShouldShim() ? m_GraphicsSystemInfo.supportsSetConstantBuffer : base.supportsSetConstantBuffer;
        public override bool hasMipMaxLevel  =>  m_GraphicsSystemInfoFields.Contains("hasMipMaxLevel") && ShouldShim() ? m_GraphicsSystemInfo.hasMipMaxLevel : base.hasMipMaxLevel;
        public override bool supportsMipStreaming  =>  m_GraphicsSystemInfoFields.Contains("supportsMipStreaming") && ShouldShim() ? m_GraphicsSystemInfo.supportsMipStreaming : base.supportsMipStreaming;
        public override bool usesLoadStoreActions  =>  m_GraphicsSystemInfoFields.Contains("usesLoadStoreActions") && ShouldShim() ? m_GraphicsSystemInfo.usesLoadStoreActions : base.usesLoadStoreActions;
    }
}
