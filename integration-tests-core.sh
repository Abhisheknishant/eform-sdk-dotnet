#!/bin/bash
./armprepareinstall.sh
dotnet restore
dotnet build --no-restore eFormSDK.sln
dotnet test --no-restore -c Release -v n eFormSDK.Integration.CoreTests/eFormSDK.Integration.CoreTests.csproj --logger:Console || travis_terminate 1;
