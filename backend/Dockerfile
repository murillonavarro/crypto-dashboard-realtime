FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CryptoDashboard.API/CryptoDashboard.API.csproj", "CryptoDashboard.API/"]
COPY ["CryptoDashboard.Core/CryptoDashboard.Core.csproj", "CryptoDashboard.Core/"]
COPY ["CryptoDashboard.Infrastructure/CryptoDashboard.Infrastructure.csproj", "CryptoDashboard.Infrastructure/"]
RUN dotnet restore "CryptoDashboard.API/CryptoDashboard.API.csproj"
COPY . .
WORKDIR "/src/CryptoDashboard.API"
RUN dotnet build "CryptoDashboard.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoDashboard.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoDashboard.API.dll"]