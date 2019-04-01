@echo off

cd ..\Bounce

dotnet pack --configuration Release
dotnet nuget push .\bin\Release\NuGet\Bounce.0.11.0-beta1.nupkg --source https://api.nuget.org/v3/index.json --api-key oy2p7awc7cczgbyrpymahlpd2wvm4kmuhejdzapffhpaka

pause