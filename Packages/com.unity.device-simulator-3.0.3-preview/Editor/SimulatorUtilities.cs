using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEditor.DeviceSimulation
{
    internal enum ResolutionScalingMode
    {
        Disabled = 0,
        FixedDpi = 1
    }

    internal enum SimulationState{ Enabled, Disabled }

    internal static class SimulatorUtilities
    {
        public static ScreenOrientation ToScreenOrientation(UIOrientation original)
        {
            switch (original)
            {
                case UIOrientation.Portrait:
                    return ScreenOrientation.Portrait;
                case UIOrientation.PortraitUpsideDown:
                    return ScreenOrientation.PortraitUpsideDown;
                case UIOrientation.LandscapeLeft:
                    return ScreenOrientation.LandscapeLeft;
                case UIOrientation.LandscapeRight:
                    return ScreenOrientation.LandscapeRight;
                case UIOrientation.AutoRotation:
                    return ScreenOrientation.AutoRotation;
            }
            throw new ArgumentException($"Unexpected value of UIOrientation {original}");
        }

        public static ScreenOrientation RotationToScreenOrientation(int angle)
        {
            ScreenOrientation orientation = ScreenOrientation.Portrait;
            if (angle > 315 || angle <= 45)
            {
                orientation = ScreenOrientation.Portrait;
            }
            else if (angle > 45 && angle <= 135)
            {
                orientation = ScreenOrientation.LandscapeRight;
            }
            else if (angle > 135 && angle <= 225)
            {
                orientation = ScreenOrientation.PortraitUpsideDown;
            }
            else if (angle > 225 && angle <= 315)
            {
                orientation = ScreenOrientation.LandscapeLeft;
            }
            return orientation;
        }

        public static bool IsLandscape(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Landscape || orientation == ScreenOrientation.LandscapeLeft ||
                orientation == ScreenOrientation.LandscapeRight)
                return true;

            return false;
        }

        public static void CheckShimmedAssemblies(List<string> shimmedAssemblies)
        {
            if (shimmedAssemblies == null || shimmedAssemblies.Count == 0)
                return;

            shimmedAssemblies.RemoveAll(string.IsNullOrEmpty);

            const string dll = ".dll";
            for (int i = 0; i < shimmedAssemblies.Count; i++)
            {
                shimmedAssemblies[i] = shimmedAssemblies[i].ToLower();
                if (!shimmedAssemblies[i].EndsWith(dll))
                {
                    shimmedAssemblies[i] += dll;
                }
            }
        }

        public static bool ShouldShim(List<string> shimmedAssemblies)
        {
            if (shimmedAssemblies == null || shimmedAssemblies.Count == 0)
                return false;

            // Here we use StackTrace to trace where the call comes from, only shim if it comes from the white listed assemblies.
            // 4 in StackTrace stands for the frames that we want to trace back up from here, as below:
            // SimulatorUtilities.ShouldShim() <-- SystemInfoSimulation/ApplicationSimulation.ShouldShim() <-- ApplicationSimulation <-- Application <-- Where the APIs are called.
            var callingAssembly = new StackTrace(4).GetFrame(0).GetMethod().Module.ToString().ToLower();
            foreach (var assembly in shimmedAssemblies)
            {
                if (callingAssembly == assembly)
                    return true;
            }
            return false;
        }
    }
}
