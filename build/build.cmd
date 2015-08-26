cls
echo off
SET DIR=%~dp0%
IF NOT EXIST "%DIR%log" MKDIR "%DIR%log"
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" /m /v:n "%DIR%msbuildtasks.proj" /target:Build /logger:FileLogger,Microsoft.Build.Engine;LogFile=%DIR%log/build.log
pause