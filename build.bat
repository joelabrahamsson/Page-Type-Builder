@echo off
for /f %%i in ("%0") do set curpath=%%~dpi
cd /d %curpath% 
Libraries\phantom\phantom.exe -f:"buildscripts\build.boo" %*