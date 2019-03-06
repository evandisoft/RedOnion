@echo off
set names=RedOnion Kerbalua
cd /D %~dp0

if not defined ksp set ksp="C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"
if not exist ksp mklink /D ksp %ksp%

for %%n in (%names%) do (
	if not exist "%%n\bin" mkdir "%%n\bin"
	if not exist "%%n\bin\Debug" mkdir "%%n\bin\Debug"
	mklink /D "ksp\GameData\%%n" "%~dp0%%n\bin\Debug"
)

