@echo off

set /p API_KEY=<nuget_api_key.txt

cd ..\Bounce

dotnet pack --configuration Release
dotnet nuget push .\bin\Release\NuGet\Bounce.0.11.0-beta2.nupkg --source https://api.nuget.org/v3/index.json --api-key %API_KEY%

pause