#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace UnityEditor.DeviceSimulation
{
    [CustomEditor(typeof(DeviceInfoImporter))]
    class DeviceInfoImporterEditor : ScriptedImporterEditor
    {
    }
}
