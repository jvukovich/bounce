@echo off

cd Bounce.Console

dotnet pack
dotnet tool install --global --add-source ./nupkg Bounce.Console

cd ..

start "" /D "%CD%\TestProject.Bounce\bin\Release\netcoreapp2.2\" cmd.exe

exit