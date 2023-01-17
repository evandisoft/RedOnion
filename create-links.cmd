@echo off
set name=RedOnion
set data=GameData\%name%
cd /D %~dp0

if defined ksp goto ksp_defined
set ksp=C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program
echo Environment variable "ksp" not defined, assuming "%ksp%"

:ksp_defined
if exist "%ksp%" goto ksp_exists
echo "%ksp%" does not exist
pause
goto:eof

:ksp_exists
if not exist ksp mklink /D ksp "%ksp%"
if not exist "%data%" mkdir "%data%"
if not exist "ksp\GameData\%name%" mklink /D "ksp\GameData\%name%" "%~dp0%data%"

