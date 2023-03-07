@echo off

if exist "..\Temp" (
    echo "Unity Need Closed!"
) else (
    @REM echo "Unity Not Open"
    rd /s "..\Library"
    mklink /d "..\Library" %~dp0ios\Library

    rd /s "..\Assets\StreamingAssets"
    mklink /d "..\Assets\StreamingAssets" %~dp0ios\StreamingAssets
)

pause