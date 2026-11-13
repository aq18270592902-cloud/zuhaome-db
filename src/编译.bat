@echo off
chcp 65001 >nul
setlocal
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set SRC=%~dp0
set OUT=%~dp0..\程序\租号么免费租号器.exe
if not exist "%CSC%" ( echo 找不到 csc.exe：%CSC% & pause & exit /b 1 )
taskkill /f /im 租号么免费租号器.exe >nul 2>&1
"%CSC%" /nologo /target:winexe /platform:anycpu /optimize+ /codepage:65001 /out:"%OUT%" /win32icon:"%SRC%app.ico" /r:System.dll /r:System.Core.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Net.Http.dll /r:System.Web.Extensions.dll "%SRC%MainForm2.cs" "%SRC%AssemblyInfo.cs"
echo.
echo exit=%ERRORLEVEL%
if exist "%OUT%" for %%F in ("%OUT%") do echo 输出: %%~fF  %%~zF bytes
pause