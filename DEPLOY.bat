:: This is just a generic "deploy to my SCP SL server" from localdev which should just work for every plugin I make without modifications. Drag and drop, should magically work :)
:: It assumes that SL_REFERENCES user environment variable is set to 2 directories above the scp sl dedicated server directory.
@echo off

cd /d "%~dp0"

:: Assuming the .sln name is the same as the .csproj name, and the csproj is in a subdirectory, it just pulls the
:: project name from the .sln name and builds appropriately.
set "PLUGIN_NAME="
for %%F in (*.sln) do set "PLUGIN_NAME=%%~nF"
if not defined PLUGIN_NAME (
    echo ERROR: No .sln file found in %CD%
    PAUSE
    exit /b 1
)

set /p ANSWER=Debug build? (d = Debug, anything else = Release): 
set "CONFIG=Release"
if /I "%ANSWER%"=="d" set "CONFIG=Debug"

echo Plugin: %PLUGIN_NAME%
echo Config: %CONFIG%

if not exist "%PLUGIN_NAME%\%PLUGIN_NAME%.csproj" (
    echo ERROR: Project file not found: %PLUGIN_NAME%\%PLUGIN_NAME%.csproj
    echo The .sln name must match the .csproj name, and the .csproj must be in a subfolder with that same name.
    PAUSE
    exit /b 1
)

:: Fixing some issues caused by my strict editorconfig - most of the time this will just be unused using statements.
dotnet format --severity error
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: dotnet format failed. Check the output above.
    PAUSE
    exit /b %ERRORLEVEL%
)

dotnet publish "%PLUGIN_NAME%\%PLUGIN_NAME%.csproj" -c %CONFIG%
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: dotnet publish failed.
    PAUSE
    exit /b %ERRORLEVEL%
)

if not exist "%PLUGIN_NAME%\bin\%CONFIG%\net48\%PLUGIN_NAME%.dll" (
    echo ERROR: %PLUGIN_NAME%.dll was not found in %PLUGIN_NAME%\bin\%CONFIG%\net48 after publishing.
    echo Check that the project targets net48 and that the assembly name matches the project name.
    PAUSE
    exit /b 1
)

Robocopy "%PLUGIN_NAME%\bin\%CONFIG%\net48" "%APPDATA%\SCP Secret Laboratory\LabAPI\plugins\global" %PLUGIN_NAME%.dll %PLUGIN_NAME%.pdb
if %ERRORLEVEL% GEQ 8 (
    echo ERROR: robocopy failed to copy the plugin files. Robocopy exit code: %ERRORLEVEL%
    PAUSE
    exit /b %ERRORLEVEL%
)

:: I don't want to restart the server for release builds in case I'm doing hotfixes or whatever.
if /I not "%CONFIG%"=="Debug" goto done

if not defined SL_REFERENCES (
    echo ERROR: SL_REFERENCES user environment variable is not set. Skipping server restart.
    PAUSE
    goto done
)

:: SL_REFERENCES is ...\SCPSL_Data\Managed; the server folder is two levels up.
for %%I in ("%SL_REFERENCES%\..\..") do set "SERVER_DIR=%%~fI"

if not exist "%SERVER_DIR%\LocalAdmin.exe" echo ERROR: LocalAdmin.exe not found in %SERVER_DIR% & echo Check that SL_REFERENCES points to the SCPSL_Data\Managed folder of the dedicated server. & PAUSE & exit /b 1

echo Stopping SCP SL dedicated server...
:: Only stops processes running from the server folder, seems reasonable enough.
powershell -NoProfile -Command "Get-Process -ErrorAction SilentlyContinue | Where-Object { $_.Path -like '%SERVER_DIR%\*' } | Stop-Process -Force"
timeout /t 3 /nobreak >nul

echo Starting SCP SL dedicated server...
start "SCP SL Server" /D "%SERVER_DIR%" "%SERVER_DIR%\LocalAdmin.exe" 7777 || (echo ERROR: Failed to start the SCP SL dedicated server. & PAUSE & exit /b 1)

:: Note this is a label (goto)
:done
echo All Done! (%PLUGIN_NAME% - %CONFIG%)