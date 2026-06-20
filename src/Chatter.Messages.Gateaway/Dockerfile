FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Chatter.Messages.Gateaway/*.csproj src/Chatter.Messages.Gateaway/
COPY src/Core/Core.Gateway/*.csproj src/Core/Core.Gateway/
COPY src/Core/Core.Observability/*.csproj src/Core/Core.Observability/
RUN dotnet restore src/Chatter.Messages.Gateaway/Chatter.Messages.Gateaway.csproj

COPY src/ src/
RUN dotnet publish src/Chatter.Messages.Gateaway/Chatter.Messages.Gateaway.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Chatter.Messages.Gateaway.dll"]