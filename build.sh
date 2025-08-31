#!/bin/bash
# Install .NET SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Add dotnet to PATH
export PATH="$HOME/.dotnet:$PATH"

# Build the project
cd backend/CryptoDashboard.API
dotnet restore
dotnet publish -c Release -o out