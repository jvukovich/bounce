#!/usr/bin/env bash

cd ..
dotnet build --configuration Release

(
  cd ./Bounce

  dotnet pack --configuration Release
  dotnet tool install --global --add-source ./bin/Release/NuGet Bounce --version 0.11.0-beta4

  IFS=''
  read -rn 1 -p 'Press enter to continue...' keyPress
  while [[ ! "${keyPress}" == '' ]]; do
    echo
    read -rn 1 -p 'Press enter to continue...' keyPress
  done
)

(
  # check for options to open test project
  testDir="$(pwd)/TestProject.Bounce/bin/Release/net6.0"

  if command -v osascript &>/dev/null; then
    osascript <<END
tell application "Terminal"
  do script "cd $testDir; bounce"
end tell
END
  elif command -v xterm &>/dev/null; then
    xterm -hold -e "cd ${testDir}; bounce"
  fi
)

# return to starting dir
cd Tools
