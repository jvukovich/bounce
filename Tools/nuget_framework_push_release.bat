@echo off

set /p API_KEY=<nuget_api_key.txt

cd ..\Bounce.Framework

dotnet pack --configuration Release
dotnet nuget push .\bin\Release\NuGet\Bounce.Framework.0.11.0-beta4.nupkg --source https://api.nuget.org/v3/index.json --api-key %API_KEY%

pause