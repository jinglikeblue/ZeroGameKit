# Adding a device

You can add new devices to the Device Simulator using **device definitions** and **device overlays**.

A device definition is a text file with the .device extension in your Unity project. It contains JSON that describes the properties of a device.

A device overlay is an image that contains the border of the device screen, together with notches, punchouts and any other additions to the screen rectangle. You can optionally use it with a device definition to visualise how hardware elements obstruct the device screen, and to determine when touch inputs will fail as a result.

## Schema

Device definitions must adhere to following schema: 


```csharp
[Serializable]
public class DeviceInfo
{
    public string friendlyName;             // [Required] Name that will be shown in UI.
    public int version;                     // [Required] Device definition file version. Currently version is 1.
    public ScreenData[] screens;            // [Required] Screen related data. Must Contain at least one screen. Support for multiple screens is not yet implemented.
    public SystemInfoData systemInfo;       // [Required] Values returned by UnityEngine.SystemInfo. Contains a single required field: operatingSystem.
}

[Serializable]
public class ScreenPresentation
{
    public string overlayPath;            // [Optional] Relative path from *.device file to an image that will be used as device overlay.
    public Vector4 borderSize;            // [Optional] Pixel distance from overlay image border to where the screen begins.
    public float cornerRadius;           // [Optional] Pixel size of corner radius
}

[Serializable]
public class ScreenData
{
    public int width;                                // [Required] Value returned by UnityEngine.Screen.width in portrait orientation.
    public int height;                               // [Required] Value returned by UnityEngine.Screen.height in portrait orientation.
    public int navigationBarHeight;                  // [Optional] Pixel height of the on-screen Android navigation bar, which appears on some devices in non-fullscreen mode.
    public float dpi;                                // [Required] Value returned by UnityEngine.Screen.dpi.
    public OrientationData[] orientations;           // [Optional] Defines which orientations are supported on the device. If this field is missing, all orientations will be supported.
    public ScreenPresentation presentation;          // [Optional] Data for drawing an overlay with device borders, notches and other irregularities baked in.
}

[Serializable]
public class OrientationData
{
    public ScreenOrientation orientation;            // [Required in OrientationData] Supported orientation
    public Rect safeArea;                            // [Optional] Value returned by UnityEngine.Screen.safeArea in full resolution. Is this field is missing, assuming that entire screen is safe.
    public Rect[] cutouts;                           // [Optional] Value returned by UnityEngine.Screen.cutouts in full resolution.
}

[Serializable]
public class SystemInfoData                                  // Fields map to UnityEngine.SystemInfo. All fields are optional except operatingSystem.
{
    public string deviceModel;
    public DeviceType deviceType;
    public string operatingSystem;                           // [Required] Must contain either Android or iOS (case-insensitive) somewhere in the string.
    public OperatingSystemFamily operatingSystemFamily;
    public int processorCount;
    public int processorFrequency;
    public string processorType;
    public bool supportsAccelerometer;
    public bool supportsAudio;
    public bool supportsGyroscope;
    public bool supportsLocationService;
    public bool supportsVibration;
    public int systemMemorySize;
    public string unsupportedIdentifier;
    public GraphicsSystemInfoData[] graphicsDependentData;   // [Optional] Defines which graphics APIs are supported on the device.
}

[Serializable]
public class GraphicsSystemInfoData                          // Fields map to UnityEngine.SystemInfo.
{
    public GraphicsDeviceType graphicsDeviceType;            // [Required in GraphicsSystemInfoData] Supported graphics API.
    public int graphicsMemorySize;
    public string graphicsDeviceName;
    public string graphicsDeviceVendor;
    public int graphicsDeviceID;
    public int graphicsDeviceVendorID;
    public bool graphicsUVStartsAtTop;
    public string graphicsDeviceVersion;
    public int graphicsShaderLevel;
    public bool graphicsMultiThreaded;
    public RenderingThreadingMode renderingThreadingMode;
    public bool hasHiddenSurfaceRemovalOnGPU;
    public bool hasDynamicUniformArrayIndexingInFragmentShaders;
    public bool supportsShadows;
    public bool supportsRawShadowDepthSampling;
    public bool supportsMotionVectors;
    public bool supports3DTextures;
    public bool supports2DArrayTextures;
    public bool supports3DRenderTextures;
    public bool supportsCubemapArrayTextures;
    public CopyTextureSupport copyTextureSupport;
    public bool supportsComputeShaders;
    public bool supportsGeometryShaders;
    public bool supportsTessellationShaders;
    public bool supportsInstancing;
    public bool supportsHardwareQuadTopology;
    public bool supports32bitsIndexBuffer;
    public bool supportsSparseTextures;
    public int supportedRenderTargetCount;
    public bool supportsSeparatedRenderTargetsBlend;
    public int supportedRandomWriteTargetCount;
    public int supportsMultisampledTextures;
    public bool supportsMultisampleAutoResolve;
    public int supportsTextureWrapMirrorOnce;
    public bool usesReversedZBuffer;
    public NPOTSupport npotSupport;
    public int maxTextureSize;
    public int maxCubemapSize;
    public int maxComputeBufferInputsVertex;
    public int maxComputeBufferInputsFragment;
    public int maxComputeBufferInputsGeometry;
    public int maxComputeBufferInputsDomain;
    public int maxComputeBufferInputsHull;
    public int maxComputeBufferInputsCompute;
    public int maxComputeWorkGroupSize;
    public int maxComputeWorkGroupSizeX;
    public int maxComputeWorkGroupSizeY;
    public int maxComputeWorkGroupSizeZ;
    public bool supportsAsyncCompute;
    public bool supportsGraphicsFence;
    public bool supportsAsyncGPUReadback;
    public bool supportsRayTracing;
    public bool supportsSetConstantBuffer;
    public bool hasMipMaxLevel;
    public bool supportsMipStreaming;
    public bool usesLoadStoreActions;
}
```

Schema errors will appear in the inspector of the device file.

## Example: minimal device definition

Here is an example of a device definition containing only required fields. Because no orientation data is provided, we assume that all orientations are supported and that safe area covers the entire screen.

```json
{
    "friendlyName": "Minimal Device",
    "version": 1,
    "screens": [
        {
            "width": 1080,
            "height": 1920,
            "dpi": 450.0
        }
    ],
    "systemInfo": {
        "operatingSystem": "Android"
    }
}
```

### Example: complete device definition

Here is an example of a device definition with all available fields set.

```json
{
    "friendlyName": "Apple iPhone XR",
    "version": 1,
    "screens": [
        {
            "width": 828,
            "height": 1792,
            "navigationBarHeight": 0,
            "dpi": 326.0,
            "orientations": [
                {
                    "orientation": 1,
                    "safeArea": {
                        "serializedVersion": "2",
                        "x": 0.0,
                        "y": 68.0,
                        "width": 828.0,
                        "height": 1636.0
                    },
                    "cutouts": [
                        {
                            "serializedVersion": "2",
                            "x": 184.0,
                            "y": 1726.0,
                            "width": 460.0,
                            "height": 66.0
                        }
                    ]
                },
                {
                    "orientation": 3,
                    "safeArea": {
                        "serializedVersion": "2",
                        "x": 88.0,
                        "y": 42.0,
                        "width": 1616.0,
                        "height": 786.0
                    },
                    "cutouts": [
                        {
                            "serializedVersion": "2",
                            "x": 0.0,
                            "y": 184.0,
                            "width": 66.0,
                            "height": 460.0
                        }
                    ]
                },
                {
                    "orientation": 4,
                    "safeArea": {
                        "serializedVersion": "2",
                        "x": 88.0,
                        "y": 42.0,
                        "width": 1616.0,
                        "height": 786.0
                    },
                    "cutouts": [
                        {
                            "serializedVersion": "2",
                            "x": 1726.0,
                            "y": 184.0,
                            "width": 66.0,
                            "height": 460.0
                        }
                    ]
                }
            ],
            "presentation": {
                "overlayPath": "Apple iPhone 11_Overlay.png",
                "borderSize": {
                    "x": 51.0,
                    "y": 51.0,
                    "z": 51.0,
                    "w": 51.0
                },
                "cornerRadius": 116.0
            }
        }
    ],
    "systemInfo": {
        "deviceModel": "iPhone11,8",
        "deviceType": 1,
        "operatingSystem": "iOS 12.0",
        "operatingSystemFamily": 0,
        "processorCount": 6,
        "processorFrequency": 0,
        "processorType": "arm64e",
        "supportsAccelerometer": true,
        "supportsAudio": true,
        "supportsGyroscope": true,
        "supportsLocationService": true,
        "supportsVibration": true,
        "systemMemorySize": 2813,
        "unsupportedIdentifier": "n/a",
        "graphicsDependentData": [
            {
                "graphicsDeviceType": 16,
                "graphicsMemorySize": 1024,
                "graphicsDeviceName": "Apple A12 GPU",
                "graphicsDeviceVendor": "Apple",
                "graphicsDeviceID": 0,
                "graphicsDeviceVendorID": 0,
                "graphicsUVStartsAtTop": true,
                "graphicsDeviceVersion": "Metal",
                "graphicsShaderLevel": 50,
                "graphicsMultiThreaded": true,
                "renderingThreadingMode": 0,
                "hasHiddenSurfaceRemovalOnGPU": true,
                "hasDynamicUniformArrayIndexingInFragmentShaders": true,
                "supportsShadows": true,
                "supportsRawShadowDepthSampling": true,
                "supportsMotionVectors": true,
                "supports3DTextures": true,
                "supports2DArrayTextures": true,
                "supports3DRenderTextures": true,
                "supportsCubemapArrayTextures": true,
                "copyTextureSupport": 31,
                "supportsComputeShaders": true,
                "supportsGeometryShaders": false,
                "supportsTessellationShaders": true,
                "supportsInstancing": true,
                "supportsHardwareQuadTopology": false,
                "supports32bitsIndexBuffer": true,
                "supportsSparseTextures": false,
                "supportedRenderTargetCount": 8,
                "supportsSeparatedRenderTargetsBlend": true,
                "supportedRandomWriteTargetCount": 8,
                "supportsMultisampledTextures": 1,
                "supportsMultisampleAutoResolve": false,
                "supportsTextureWrapMirrorOnce": 0,
                "usesReversedZBuffer": true,
                "npotSupport": 2,
                "maxTextureSize": 16384,
                "maxCubemapSize": 16384,
                "maxComputeBufferInputsVertex": 8,
                "maxComputeBufferInputsFragment": 8,
                "maxComputeBufferInputsGeometry": 0,
                "maxComputeBufferInputsDomain": 8,
                "maxComputeBufferInputsHull": 8,
                "maxComputeBufferInputsCompute": 8,
                "maxComputeWorkGroupSize": 1024,
                "maxComputeWorkGroupSizeX": 1024,
                "maxComputeWorkGroupSizeY": 1024,
                "maxComputeWorkGroupSizeZ": 1024,
                "supportsAsyncCompute": false,
                "supportsGraphicsFence": true,
                "supportsAsyncGPUReadback": true,
                "supportsRayTracing": false,
                "supportsSetConstantBuffer": true,
                "hasMipMaxLevel": true,
                "supportsMipStreaming": true,
                "usesLoadStoreActions": true,
                "supportedTextureFormats": [1, 2, 3, 4, 5],
                "supportedRenderTextureFormats": [1, 2, 3, 4, 5],
                "ldrGraphicsFormat": 59,
                "hdrGraphicsFormat": 74
            }
        ]
    }
}
```
