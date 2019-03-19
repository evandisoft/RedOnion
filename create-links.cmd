@echo off
set name=RedOnion
cd /D %~dp0

if not defined ksp set ksp="C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"
REM if not exist ksp mklink /D ksp %ksp%
if not exist ..\ksp mklink /D ..\ksp %ksp%

if not exist "%name%\bin" mkdir "%name%\bin"
if not exist "%name%\bin\Debug" mkdir "%name%\bin\Debug"
if not exist "..\ksp\GameData\%name%" mklink /D "..\ksp\GameData\%name%" "%~dp0%name%\bin\Debug"


