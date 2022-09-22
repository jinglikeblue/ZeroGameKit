using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.DeviceSimulator.Editor.Tests.Utilities
{
    internal static class ScreenTestUtilities
    {
        public static Dictionary<ScreenOrientation, int> OrientationRotation =
            new Dictionary<ScreenOrientation, int>
        {
            {ScreenOrientation.Portrait, 0},
            {ScreenOrientation.PortraitUpsideDown, 180},
            {ScreenOrientation.LandscapeLeft, 270},
            {ScreenOrientation.LandscapeRight, 90}
        };

        public static Dictionary<ScreenOrientation, UIOrientation> ScreenOrientationToUI =
            new Dictionary<ScreenOrientation, UIOrientation>
        {
            {ScreenOrientation.Portrait, UIOrientation.Portrait},
            {ScreenOrientation.PortraitUpsideDown, UIOrientation.PortraitUpsideDown},
            {ScreenOrientation.LandscapeLeft, UIOrientation.LandscapeLeft},
            {ScreenOrientation.LandscapeRight, UIOrientation.LandscapeRight}
        };

        public static ScreenOrientation[] ExplicitOrientations =
        {
            ScreenOrientation.Portrait, ScreenOrientation.PortraitUpsideDown, ScreenOrientation.LandscapeLeft,
            ScreenOrientation.LandscapeRight
        };

        public static void SetScreenAutoOrientation(ScreenOrientation orientation, bool value)
        {
            switch (orientation)
            {
                case ScreenOrientation.Portrait:
                    Screen.autorotateToPortrait = value;
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    Screen.autorotateToPortraitUpsideDown = value;
                    break;
                case ScreenOrientation.LandscapeLeft:
                    Screen.autorotateToLandscapeLeft = value;
                    break;
                case ScreenOrientation.LandscapeRight:
                    Screen.autorotateToLandscapeRight = value;
                    break;
                default:
                    throw new ArgumentException(
                        $"Supported values are Portrait, PortraitUpsideDown, LandscapeLeft or LandscapeRight, but {orientation} provided.");
            }
        }

        public static bool IsLandscape(this ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.Portrait:
                case ScreenOrientation.PortraitUpsideDown:
                    return false;
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                    return true;
                default:
                    throw new ArgumentException(
                        $"Supported values are Portrait, PortraitUpsideDown, LandscapeLeft or LandscapeRight, but {orientation} provided.");
            }
        }

        public static bool IsLandscape(this UIOrientation orientation)
        {
            switch (orientation)
            {
                case UIOrientation.Portrait:
                case UIOrientation.PortraitUpsideDown:
                    return false;
                case UIOrientation.LandscapeLeft:
                case UIOrientation.LandscapeRight:
                    return true;
                default:
                    throw new ArgumentException(
                        $"Supported values are Portrait, PortraitUpsideDown, LandscapeLeft or LandscapeRight, but {orientation} provided.");
            }
        }
    }
}
