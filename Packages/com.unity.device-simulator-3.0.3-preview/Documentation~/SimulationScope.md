# Simulation scope

Simulation of [SystemInfo](https://docs.unity3d.com/Documentation/ScriptReference/SystemInfo.html) and [Application](https://docs.unity3d.com/Documentation/ScriptReference/Application.html) classes can be very resource intensive in cases where members of these classes are called often. To avoid this performance cost, SystemInfo and Application class simulation can be disabled in `Project Settings -> Device Simulator`.

The scope in which calls to [SystemInfo](https://docs.unity3d.com/Documentation/ScriptReference/SystemInfo.html) and [Application](https://docs.unity3d.com/Documentation/ScriptReference/Application.html) return simulated results is limited in order to preserve functionality of the editor and editor extensions. Simulation can be turned ON or OFF for each assembly. By default simulation is ON for scripts that are outside of any custom assemblies.

To simulate SystemInfo and Application class behavior in custom assemblies, assembly names have to be added to the list found in `Project Settings -> Device Simulator`.

[Screen](https://docs.unity3d.com/Documentation/ScriptReference/Screen.html) simulation is not limited by assembly.

## Simulated API List

### [Screen](https://docs.unity3d.com/Documentation/ScriptReference/Screen.html)
- autorotateToLandscapeLeft
- autorotateToLandscapeRight
- autorotateToPortrait
- autorotateToPortraitUpsideDown
- currentResolution
- cutouts
- dpi
- fullScreen
- fullScreenMode
- height
- orientation
- safeArea
- width

### [SystemInfo](https://docs.unity3d.com/Documentation/ScriptReference/SystemInfo.html)
- copyTextureSupport
- deviceModel
- deviceType
- graphicsDeviceID
- graphicsDeviceName
- graphicsDeviceType
- graphicsDeviceVendor
- graphicsDeviceVendorID
- graphicsDeviceVersion
- graphicsMemorySize
- graphicsMultiThreaded
- graphicsShaderLevel
- graphicsUVStartsAtTop
- hasDynamicUniformArrayIndexingInFragmentShaders
- hasHiddenSurfaceRemovalOnGPU
- hasMipMaxLevel
- hdrDisplaySupportFlags
- maxComputeBufferInputsCompute
- maxComputeBufferInputsDomain
- maxComputeBufferInputsFragment
- maxComputeBufferInputsGeometry
- maxComputeBufferInputsHull
- maxComputeBufferInputsVertex
- maxComputeWorkGroupSize
- maxComputeWorkGroupSizeX
- maxComputeWorkGroupSizeY
- maxComputeWorkGroupSizeZ
- maxCubemapSize
- maxTextureSize
- minConstantBufferOffsetAlignment
- npotSupport
- operatingSystem
- operatingSystemFamily
- processorCount
- processorFrequency
- processorType
- renderingThreadingMode
- supportedRandomWriteTargetCount
- supportedRenderTargetCount
- supports2DArrayTextures
- supports32bitsIndexBuffer
- supports3DRenderTextures
- supports3DTextures
- supportsAccelerometer
- supportsAsyncCompute
- supportsAsyncGPUReadback
- supportsAudio
- supportsComputeShaders
- supportsCubemapArrayTextures
- supportsGeometryShaders
- supportsGpuRecorder
- supportsGraphicsFence
- supportsGyroscope
- supportsHardwareQuadTopology
- supportsInstancing
- supportsLocationService
- supportsMipStreaming
- supportsMotionVectors
- supportsMultisampleAutoResolve
- supportsMultisampledTextures
- supportsRawShadowDepthSampling
- supportsRayTracing
- supportsSeparatedRenderTargetsBlend
- supportsSetConstantBuffer
- supportsShadows
- supportsSparseTextures
- supportsTessellationShaders
- supportsTextureWrapMirrorOnce
- supportsVibration
- systemMemorySize
- unsupportedIdentifier
- usesLoadStoreActions
- usesReversedZBuffer
<!-- GetGraphicsFormat\
SupportsRenderTextureFormat\
SupportsTextureFormat -->

### [Application](https://docs.unity3d.com/Documentation/ScriptReference/Application.html)
- internetReachability
- isConsolePlatform
- isEditor
- isMobilePlatform
- platform
- systemLanguage
- LowMemoryCallback
