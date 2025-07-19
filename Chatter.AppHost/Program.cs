var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder
    .AddPostgres("postgres", username, password, 5432)
    .AddDatabase("MessagesDb");

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var keycloakGateway = builder.AddProject<Projects.Chatter_Keycloak_Gateaway>("keycloakGateaway")
    .WithReference(keycloak)
    .WithHttpEndpoint(name: "keycloakGateaway", port: 7070);

var messagesGateway = builder.AddProject<Projects.Chatter_Messages_Gateaway>("messagesGateaway")
    .WithHttpEndpoint(name: "messagesGateaway", port: 7071);

var messagesApi = builder.AddProject<Projects.Chatter_Messages_Presentation>("messagesApi")
    .WithReference(postgres)
    .WithHttpEndpoint(name: "messagesApi", port: 7072);

builder.Build().Run();