@echo off
set name=RedOnion
set data=GameData\%name%
cd /D %~dp0

if not defined ksp set ksp="C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"
REM if not exist ksp mklink /D ksp %ksp%
if not exist ..\ksp mklink /D ..\ksp %ksp%

if not exist "%data%" mkdir "%data%"
if not exist "..\ksp\GameData\%name%" mklink /D "..\ksp\GameData\%name%" "%~dp0%data%"


