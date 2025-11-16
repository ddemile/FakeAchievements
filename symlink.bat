@echo off
openfiles >nul 2>&1
if %errorlevel% NEQ 0 (
    powershell start-process -verb runas '%0'
    exit /b
)

set /p path_input=Rain World game folder: 
mklink /d "%~dp0\lib\RainWorldManaged" "%path_input%\RainWorld_Data\Managed"
mklink /d "%path_input%\RainWorld_Data\StreamingAssets\mods\FakeAchievements" "%~dp0mod"
pause