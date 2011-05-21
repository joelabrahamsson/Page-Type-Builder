@echo off
IF "%1"=="" GOTO ReleaseNameError
for /f %%i in ("%0") do set curpath=%%~dpi
cd /d %curpath% 
Libraries\phantom\phantom.exe -f:"buildscripts\build.boo" release -a:releasename=%1
GOTO:EOF
:ReleaseNameError
ECHO Invalid argument: A release name must be specified