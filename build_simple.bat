@echo off
title Simple Build

echo Simple build without single file...
dotnet publish -c Release -o "publish_simple"

echo.
echo Try running: publish_simple\ClientManagerMaterial.exe
pause