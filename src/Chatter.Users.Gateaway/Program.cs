using Core.Gateway;
using Core.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGateway(builder.Configuration);
builder.AddObservability("users-gateway");

var app = builder.Build();

app.UseGateway();
app.Run();