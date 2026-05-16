FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Chatter.Users.Presentation/*.csproj src/Chatter.Users.Presentation/
COPY src/Chatter.Users.Application/*.csproj src/Chatter.Users.Application/
COPY src/Chatter.Users.DataAccess/*.csproj src/Chatter.Users.DataAccess/
COPY src/Chatter.Shared.Domain/*.csproj src/Chatter.Shared.Domain/
COPY src/Chatter.Shared.Context/*.csproj src/Chatter.Shared.Context/
COPY src/Chatter.Shared.KeycloakEventsProcessor/*.csproj src/Chatter.Shared.KeycloakEventsProcessor/
COPY src/Core/Core.CQRS/*.csproj src/Core/Core.CQRS/
COPY src/Core/Core.Web/*.csproj src/Core/Core.Web/
COPY src/Core/Core.Migrator/*.csproj src/Core/Core.Migrator/
COPY src/Core/Core.Observability/*.csproj src/Core/Core.Observability/
COPY src/Core/Core.ResultPattern/*.csproj src/Core/Core.ResultPattern/
COPY src/Core/Core.DomainTypes/*.csproj src/Core/Core.DomainTypes/
COPY src/Core/Core.DateTimeProvider/*.csproj src/Core/Core.DateTimeProvider/
COPY src/Core/Core.DataAccessTypes/*.csproj src/Core/Core.DataAccessTypes/
COPY src/Core/Core.Infrastructure/*.csproj src/Core/Core.Infrastructure/
COPY src/Core/Core.KafkaProducer/*.csproj src/Core/Core.KafkaProducer/
COPY src/Core/Core.KafkaConsumer/*.csproj src/Core/Core.KafkaConsumer/
COPY src/Core/Core.Outbox/*.csproj src/Core/Core.Outbox/
COPY src/Core/Core.Pager/*.csproj src/Core/Core.Pager/
COPY src/Core/Core.Logger/*.csproj src/Core/Core.Logger/
COPY src/Core/Core.KeycloakService/*.csproj src/Core/Core.KeycloakService/
RUN dotnet restore src/Chatter.Users.Presentation/Chatter.Users.Presentation.csproj

COPY src/ src/
RUN dotnet publish src/Chatter.Users.Presentation/Chatter.Users.Presentation.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Chatter.Users.Presentation.dll"]