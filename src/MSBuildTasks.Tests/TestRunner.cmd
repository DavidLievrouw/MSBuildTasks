cls
echo off
SET DIR=%~dp0%
"%WINDIR%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" "%DIR%\GetVersionParts\GetVersionPartsTests.proj"
pause