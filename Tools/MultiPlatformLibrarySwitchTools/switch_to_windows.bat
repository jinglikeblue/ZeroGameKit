@echo off

if exist "..\Temp" (
    echo "Unity Need Closed!"
) else (
    @REM echo "Unity Not Open"
    rd /s "..\Library"
    mklink /d "..\Library" %~dp0windows\Library

    rd /s "..\Assets\StreamingAssets"
    mklink /d "..\Assets\StreamingAssets" %~dp0windows\StreamingAssets
)

pause