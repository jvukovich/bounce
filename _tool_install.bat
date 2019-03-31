@echo off

cd Bounce.Console

dotnet pack
dotnet tool install --global --add-source ./nupkg Bounce.Console

start "" /D "TestProject.Bounce\bin\Release\netcoreapp2.2" cmd.exe

exit