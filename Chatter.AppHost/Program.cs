var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder
    .AddPostgres("postgres", username, password, 5432)
    .WithDataVolume(isReadOnly: false);

var messagesDb = postgres.AddDatabase("MessagesDb");
var hangfireDb = postgres.AddDatabase("HangfireDb");

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

var keycloakGateway = builder.AddProject<Projects.Chatter_Keycloak_Gateaway>("keycloakGateaway")
    .WithReference(keycloak)
    .WithHttpEndpoint(name: "keycloakGateaway", port: 7070);

var messagesGateway = builder.AddProject<Projects.Chatter_Messages_Gateaway>("messagesGateaway")
    .WithHttpEndpoint(name: "messagesGateaway", port: 7071);

var elasticsearch = builder
    .AddElasticsearch("elasticsearch", port: 9200)
    .WithEnvironment("xpack.security.enabled", "false");

var kibana = builder
    .AddContainer("kibana", "kibana", "8.15.3")
    .WithReference(elasticsearch)
    .WithEndpoint(5601, 5601);

var messagesApi = builder.AddProject<Projects.Chatter_Messages_Presentation>("messagesApi")
    .WithReference(postgres)
    .WithReference(elasticsearch)
    .WithHttpEndpoint(name: "messagesApi", port: 7072)
    .WaitFor(elasticsearch)
    .WaitFor(messagesDb);

var syncUsersJob = builder.AddProject<Projects.Chatter_SyncKeycloakEventsJob>("syncUsersJob")
    .WithReference(hangfireDb)
    .WithReference(messagesDb)
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch)
    .WaitFor(hangfireDb)
    .WaitFor(messagesDb);


builder.Build().Run();