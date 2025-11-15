<<<<<<< HEAD
@echo off
title Final Build with Installer

echo 1. Building application...
dotnet publish -c Release -o "publish_simple"

echo 2. Creating installer...
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" "Setup\setup.iss"

echo.
echo SUCCESS! Installer created: Output\ClientManagerSetup.exe
echo.
=======
@echo off
title Final Build with Installer

echo 1. Building application...
dotnet publish -c Release -o "publish_simple"

echo 2. Creating installer...
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" "Setup\setup.iss"

echo.
echo SUCCESS! Installer created: Output\ClientManagerSetup.exe
echo.
>>>>>>> c76f20a82ca04839e9b3dd7f251cfe2307e5af27
pause