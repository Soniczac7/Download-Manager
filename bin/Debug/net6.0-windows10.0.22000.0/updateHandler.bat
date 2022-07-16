
@echo off
cls
title Download Manager Update Handler
echo.
echo This window is to start the Download Manager Installer with arguments "--update".
echo Once the process has exited it will send the exit code to Download Manager to log.
echo Please do not close this window. It will automatically close once setup has completed.
echo.
cd %1
set ERRORLEVEL=0
echo %1%2%3%4%5%6%7%8%9\DownloadManagerInstaller.exe --update
start "" /wait %1%2%3%4%5%6%7%8%9\DownloadManagerInstaller.exe --update
if %ERRORLEVEL% == 0 goto success
if %ERRORLEVEL%==1 goto success1
if %$ERRORLEVEL%==2 goto error1
if %ERRORLEVEL%==3 goto error2
if %ERRORLEVEL%==4 goto error3
if %ERRORLEVEL%==5 goto error4
goto error5
cls
echo.
echo Update error: An unknown error occurred while checking for updates.
echo.
exit /b %ERRORLEVEL%

:success
cls
echo.
echo Update Successful.
echo.
exit /b 0

:success1
cls
echo.
echo No update avaliable.
echo.
exit /b 1

:error1
cls
echo.
echo Update error: Setup exited before the installation could complete.
echo.
exit /b 2

:error2
cls
echo.
echo Update error: The update XML file is malformed.
echo.
exit /b 3

:error3
cls
echo.
echo Update error: The update failed due to an error during the installation process.
echo.
exit /b 4

:error4
cls
echo.
echo Update error: Setup could not retrieve the existing Download Manager installation. Ensure setup is in the same path as Download Manager and try again.
echo.
exit /b 5

:error5
cls
echo.
echo Update error: An unknown error occurred while checking for updates.
echo.
exit /b 6