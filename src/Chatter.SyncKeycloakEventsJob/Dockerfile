FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Chatter.SyncKeycloakEventsJob/*.csproj src/Chatter.SyncKeycloakEventsJob/
COPY src/Chatter.Shared.Domain/*.csproj src/Chatter.Shared.Domain/
COPY src/Core/Core.DomainTypes/*.csproj src/Core/Core.DomainTypes/
COPY src/Core/Core.Infrastructure/*.csproj src/Core/Core.Infrastructure/
COPY src/Core/Core.KeycloakService/*.csproj src/Core/Core.KeycloakService/
COPY src/Core/Core.Logger/*.csproj src/Core/Core.Logger/
COPY src/Core/Core.Observability/*.csproj src/Core/Core.Observability/
COPY src/Core/Core.DataAccessTypes/*.csproj src/Core/Core.DataAccessTypes/
COPY src/Core/Core.DateTimeProvider/*.csproj src/Core/Core.DateTimeProvider/
RUN dotnet restore src/Chatter.SyncKeycloakEventsJob/Chatter.SyncKeycloakEventsJob.csproj

COPY src/ src/
RUN dotnet publish src/Chatter.SyncKeycloakEventsJob/Chatter.SyncKeycloakEventsJob.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Chatter.SyncKeycloakEventsJob.dll"]