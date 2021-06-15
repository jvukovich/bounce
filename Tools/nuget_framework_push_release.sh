#!/usr/bin/env bash

read -p 'nuget_api_key.txt: ' apiKey

(
  cd ..
  dotnet build --configuration Release

  cd ./Bounce.Framework

  dotnet pack --configuration Release
  dotnet nuget push ./bin/Release/NuGet/Bounce.Framework.0.11.0-beta2.nupkg --source https://api.nuget.org/v3/index.json --api-key "${apiKey}"
)
