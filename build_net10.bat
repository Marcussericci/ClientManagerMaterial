<<<<<<< HEAD
@echo off
title Build for .NET 10.0

echo Building for .NET 10.0...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "publish_net10"

echo.
echo Try running: publish_net10\ClientManagerMaterial.exe
echo If it works, we'll update the installer!
=======
@echo off
title Build for .NET 10.0

echo Building for .NET 10.0...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "publish_net10"

echo.
echo Try running: publish_net10\ClientManagerMaterial.exe
echo If it works, we'll update the installer!
>>>>>>> c76f20a82ca04839e9b3dd7f251cfe2307e5af27
pause