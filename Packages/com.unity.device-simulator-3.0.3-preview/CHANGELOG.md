# Changelog

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.0.3-preview] - 2021-09-28

### Fixed
- Case 1333953 IMGUI button presses don't get registered while in a Device Simulator.

## [3.0.2-preview] - 2021-09-08

### Fixed
- Case 1355553 All assets are reimported when installing Device Simulator package for the first time.

## [3.0.1-preview] - 2021-06-08

### Fixed
- Application.isEditor is always false when simulator is active. Caused build errors.

## [3.0.0-preview] - 2021-04-06

### Added
- Added DeviceSimulatorPlugin class - a new way to extend the Device Simulator.
- Added new devices.

### Changed
- Redesigned how custom devices are added to the Device Simulator.

### Removed
- Preferences for Device Simulator were removed due to changes in how custom devices are loaded.
- Device Specifications panel removed fromm the Control Panel.
- Screen Settings panel removed from the Control Panel.

### Fixed
- Case 1295407 The input is detected when clicking on notch.
- Case 1217736 Fix resolution, safe area, and cutouts when simulating Android devices in windowed mode.

## [2.2.4-preview] - 2020-11-30

### Fixed
- Case 1253362 Screen.SafeArea returns different values with the same device when maximizing and unmaximizing the Device Simulator window.
- Case 1227475 Device Simulator does not fit into the window when minimizing the Simulator window while 'Fit To Screen' is turned on.
- Case 1284136 Device Simulator window is empty and NullReferenceException is thrown when Input System package is installed.
- Case 1269790 UnauthorizedAccessException errors are thrown when selecting Device Simulator tab in the Project Settings.

## [2.2.3-preview] - 2020-07-29

### Fixed
- Case 1258816 Installing Device Simulator package results in 'TwoPaneSplitView' error.
- Case 1247506 [Device Simulator] resets Application Settings when entering Play Mode.
- Case 1228680 An error is thrown when trying to retrieve/load Device Simulator window icon.
- Failing to create RenderTexture when resolution is set to 0.
- Fixed simulator window style in personal theme. New icons and matching divider colors.

## [2.2.2-preview] - 2020-04-22

### Changed
- Only left mouse click is used for touch simulation. Right click used to work. That was not intended and caused issues when both buttons were pressed.

### Fixed
- Case 1215351 Screen is flipped vertically when the editor is using OpenGL.
- IMGUI input is no longer registered in the wrong place.
- Old Input touches transition to Stationary phase. Used to get stuck in the Moved phase, even when the cursor was stationary.

## [2.2.1-preview] - 2020-03-28

### Fixed
-  Unity.DeviceSimulator.Tests.Common assembly is included for all platforms, this causes builds to fail.

## [2.2.0-preview] - 2020-03-26

### Added
-  Support for the new Input System. One finger touch.

## [2.1.1-preview] - 2020-03-20

### Fixed
-  Case 1229098 Device simulator 2.1.0 has compilation errors in SimulatorWindow class in 2020.1.0b1

## [2.1.0-preview] - 2020-03-04

### Added
- Serializing extensions during domain reload.

### Changed
- Completed a significant refactoring of how simulated device is rendered. Old code got way too messy.

### Fixed
- Removed dependency on NUnit.Framework.

## [2.0.0-preview] - 2019-12-05

### Added
- Over 50 new devices.
- Searchable list of devices.
- Simulation of UnityEngine.Application functionality: internetReachability, isConsolePlatform, isEditor, isMobilePlatform, platform, systemLanguage, LowMemoryCallback.
- device.json file validation. Devices that don't pass validation are ignored.
- Documentation for device.json format.
- Documentation for setting up SystemInfo and Application class simulation.

### Changed
- device.json format. The new format is incompatible with the old one.

### Fixed
- Null reference exception when certain fields were omitted from device.json file.
- Main camera transform affecting and corrupting device rendering.
- Control panel stays hidden after domain reload.

## [1.3.0-preview] - 2019-11-15

### Added
- Support loading render doc from the Simulator view
- Safe area highlight color and line width can now be configured in Preferences
- Simulator info panel can now be hidden

### Removed
- Removed Player Settings section from the Simulator info panel

### Fixed
- Preview image is no longer darker than game view with linear color space
- Top of the device can now be scrolled to when the Simulator view is zoomed in


## [1.2.0-preview] - 2019-10-11
### Added
- `Preferences -> Device Simulator` to set the customised device directory

## [1.1.0-preview] - 2019-09-26
### Added
- `Project Settings -> Device Simulator`
- SystemInfo simulation from arbitrary assemblies. Assembly list in Project Settings
- Spacer lines between foldouts in the control panel for visual clarity

### Changed
- `Use Player Settings` toggle was renamed to `Override Player Settings`. Its function was also inverted
- New device rotation icons

### Fixed
- Control panel now has a scroll view. UI elements no longer overlap each other when they don't fit
- UI elements from control panel no longer bleed into device panel
- Fixed "Layout update is struggling to process current layout (consider simplifying to avoid recursive layout)" error

## [1.0.0-preview] - 2019-09-23
### Added
- Simulating SystemInfo if called directly from Assembly-CSharp
- Warning is shown if simulator window is not active, which happens when some other simulator or game window owns rendering
- Simulating windowed mode on Android
- Simulating Screen.cutouts
- Highlight Safe Area toggle
- Documentation

### Changed
- Fit to Screen is now a toggle
- DeviceInfo format was changed to include multiple graphics APIs

## [0.1.0-preview.1] - 2019-08-21

### This is the first release of *Unity Device Simulator Package*.
