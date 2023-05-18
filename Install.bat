@echo off
cd %~dp0
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe ".\Auto Download Mover.exe"

setlocal enabledelayedexpansion

REM Define the XML file path and the key to search for
set "XML_FILE=Auto Download Mover.exe.config"
set "KEY=ServiceName"

REM Create a temporary VBScript file
echo Set xmlDoc = CreateObject("Microsoft.XMLDOM")>"temp.vbs"
echo xmlDoc.Load("%XML_FILE%")>>"temp.vbs"
echo WScript.Echo xmlDoc.SelectSingleNode("//add[@key='%KEY%']").getAttribute("value")>>"temp.vbs"

REM Use cscript to execute the VBScript and capture the output
for /f "usebackq delims=" %%G in (`cscript //nologo "temp.vbs"`) do (
    set "SERVICENAME=%%~G"
)

REM Remove leading and trailing spaces from the extracted value
set "SERVICENAME=!SERVICENAME: "=!"
set "SERVICENAME=!SERVICENAME:"=!"

sc start "%SERVICENAME%"
endlocal

REM Clean up
del "temp.vbs"
del "*.Install*"