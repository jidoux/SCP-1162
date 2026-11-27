:: All this does is delete the config for this plugin in my SCP SL dedicated server to save me a bit of time.
@echo off

cd /d "%~dp0"

:: Pulls the plugin name from the .sln name like the deploy script did.
set "PLUGIN_NAME="
for %%F in (*.sln) do set "PLUGIN_NAME=%%~nF"
if not defined PLUGIN_NAME (
    echo ERROR: No .sln file found in %CD%
    PAUSE
    exit /b 1
)

set "CONFIG_DIR=%APPDATA%\SCP Secret Laboratory\LabAPI\configs\7777\%PLUGIN_NAME%"

if not exist "%CONFIG_DIR%" (
    echo Nothing to delete, config folder not found: %CONFIG_DIR%
    PAUSE
    exit /b 0
)

echo Deleting config: %CONFIG_DIR%
rmdir /s /q "%CONFIG_DIR%"
if exist "%CONFIG_DIR%" (
    echo ERROR: Failed to delete %CONFIG_DIR%
    echo Make sure the server isn't running and holding the files open.
    PAUSE
    exit /b 1
)

echo All Done! Config deleted for %PLUGIN_NAME%. It will regenerate with defaults on the next server start.