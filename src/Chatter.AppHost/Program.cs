var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder
    .AddPostgres("postgres", username, password, 5432)
    .WithDataVolume(isReadOnly: false);

var messagesDb = postgres.AddDatabase("MessagesDb");
var usersDb = postgres.AddDatabase("UsersDb");
var hangfireDb = postgres.AddDatabase("HangfireDb");

var kafka = builder
    .AddKafka("kafka");


var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume("keycloak-data")
    .WithRealmImport("keycloak-realms")
    .WithExternalHttpEndpoints();

var loki = builder
    .AddContainer("loki", "grafana/loki", "3.4.2")
    .WithHttpEndpoint(name: "loki", port: 3100, targetPort: 3100);

var tempo = builder
    .AddContainer("tempo", "grafana/tempo", "2.6.1")
    .WithBindMount("tempo.yml", "/etc/tempo/tempo.yaml")
    .WithArgs("-config.file=/etc/tempo/tempo.yaml")
    .WithHttpEndpoint(name: "otlp-http", port: 4318, targetPort: 4318)
    .WithHttpEndpoint(name: "query", port: 3200, targetPort: 3200);

var prometheus = builder
    .AddContainer("prometheus", "prom/prometheus", "v2.53.3")
    .WithBindMount("prometheus.yml", "/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(name: "prometheus", port: 9090, targetPort: 9090);

var grafana = builder
    .AddContainer("grafana", "grafana/grafana", "11.6.0")
    .WithBindMount("grafana/provisioning", "/etc/grafana/provisioning")
    .WithEndpoint(3000, 3000)
    .WaitFor(loki)
    .WaitFor(tempo)
    .WaitFor(prometheus);

var otelCollector = builder
    .AddContainer("otel-collector", "otel/opentelemetry-collector-contrib", "0.127.0")
    .WithBindMount("otel-collector-config.yaml", "/etc/otelcol-contrib/config.yaml")
    .WithHttpEndpoint(name: "otlp-http", port: 14318, targetPort: 4318)
    .WithHttpEndpoint(name: "prometheus-metrics", port: 8889, targetPort: 8889)
    .WaitFor(loki)
    .WaitFor(tempo)
    .WaitFor(prometheus);

var otlpEndpoint = otelCollector.GetEndpoint("otlp-http");

// Use callback form so our values are written last into the env var dictionary,
// overriding Aspire's auto-injected OTEL_EXPORTER_OTLP_ENDPOINT (dashboard URL).
// Use a custom name — Aspire's DCP auto-injects OTEL_EXPORTER_OTLP_ENDPOINT
// at process launch (pointing to its own dashboard), which overwrites any
// WithEnvironment callback that sets the same key. A custom name sidesteps this.
void ConfigureOtel(EnvironmentCallbackContext ctx)
{
    ctx.EnvironmentVariables["CHATTER_OTLP_ENDPOINT"] = otlpEndpoint;
    ctx.EnvironmentVariables["CHATTER_OTLP_PROTOCOL"] = "http/protobuf";
}

var messagesGateway = builder.AddProject<Projects.Chatter_Messages_Gateaway>("messagesGateaway")
    .WithHttpEndpoint(name: "messagesGateaway", port: 7071)
    .WithEnvironment(ConfigureOtel);

var usersGateway = builder.AddProject<Projects.Chatter_Users_Gateaway>("usersGateaway")
    .WithHttpEndpoint(name: "usersGateaway", port: 7073)
    .WithEnvironment(ConfigureOtel);

var messagesApi = builder.AddProject<Projects.Chatter_Messages_Presentation>("messagesApi")
    .WithReference(postgres)
    .WithReference(kafka)
    .WithHttpEndpoint(name: "messagesApi", port: 7072)
    .WithEnvironment(ConfigureOtel)
    .WaitFor(messagesDb)
    .WaitFor(kafka);

var usersApi = builder.AddProject<Projects.Chatter_Users_Presentation>("usersApi")
    .WithReference(postgres)
    .WithReference(keycloak)
    .WithHttpEndpoint(name: "usersApi", port: 7074)
    .WithEnvironment(ConfigureOtel)
    .WaitFor(usersDb);

var syncUsersJob = builder.AddProject<Projects.Chatter_SyncKeycloakEventsJob>("syncUsersJob")
    .WithReference(keycloak)
    .WithReference(hangfireDb)
    .WithReference(messagesDb)
    .WithEnvironment(ConfigureOtel)
    .WaitFor(keycloak)
    .WaitFor(hangfireDb)
    .WaitFor(messagesDb);


builder.Build().Run();