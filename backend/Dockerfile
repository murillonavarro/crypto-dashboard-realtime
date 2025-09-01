# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos os arquivos csproj primeiro
COPY CryptoDashboard.API/*.csproj ./CryptoDashboard.API/
COPY CryptoDashboard.Core/*.csproj ./CryptoDashboard.Core/
COPY CryptoDashboard.Infrastructure/*.csproj ./CryptoDashboard.Infrastructure/

# Restaurar
RUN dotnet restore CryptoDashboard.API/CryptoDashboard.API.csproj

# Copiar o resto do código
COPY . .

# Build
RUN dotnet publish CryptoDashboard.API/CryptoDashboard.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "CryptoDashboard.API.dll"]