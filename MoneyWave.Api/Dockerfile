FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MoneyWave.Api/MoneyWave.Api.csproj", "MoneyWave.Api/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Core.WebApi/Core.WebApi.csproj", "Core.WebApi/"]
COPY ["Core.EventStoreDb/Core.EventStoreDb.csproj", "Core.EventStoreDb/"]
COPY ["Core.Serialization/Core.Serialization.csproj", "Core.Serialization/"]
COPY ["Core.Marten/Core.Marten.csproj", "Core.Marten/"]
COPY ["MoneyWave.UserAccounts/MoneyWave.UserAccounts.csproj", "MoneyWave.UserAccounts/"]
RUN dotnet restore "MoneyWave.Api/MoneyWave.Api.csproj"
COPY . .
WORKDIR "/src/MoneyWave.Api"
RUN dotnet build "MoneyWave.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MoneyWave.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoneyWave.Api.dll"]
