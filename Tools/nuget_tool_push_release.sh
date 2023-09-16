#!/usr/bin/env bash

read -p 'nuget_api_key.txt: ' apiKey

(
  cd ..
  dotnet build --configuration Release

  cd ./Bounce

  dotnet pack --configuration Release
  dotnet nuget push ./bin/Release/NuGet/Bounce.0.11.0-beta4.nupkg --source https://api.nuget.org/v3/index.json --api-key "${apiKey}"
)
