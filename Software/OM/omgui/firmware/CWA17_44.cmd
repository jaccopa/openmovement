@echo off
cd /d %~dp0
echo Bootload: %~n0
booter.exe -copy 0x2A000 4 -timeout 15 "%~n0.hex"
