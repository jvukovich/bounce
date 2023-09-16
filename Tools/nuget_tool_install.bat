@echo off

cd ..\Bounce

dotnet pack --configuration Release
dotnet tool install --global --add-source ./bin/Release/NuGet Bounce --version 0.11.0-beta4

pause

cd ..

start "" /D "%CD%\TestProject.Bounce\bin\Release\net6.0\" cmd.exe

exit
