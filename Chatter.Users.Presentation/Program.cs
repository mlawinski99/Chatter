using System.Reflection;
using Chatter.Shared.CQRS;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.Encryption;
using Chatter.Shared.KeycloakService;
using Chatter.Shared.Logger;
using Chatter.SyncKeycloakEventsJob;
using Chatter.Users.DataAccess;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var keycloakConfiguration = builder.Configuration.GetSection("Keycloak");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakConfiguration["Authority"];
        options.Audience = keycloakConfiguration["Audience"];

        options.RequireHttpsMetadata = keycloakConfiguration["RequireHttpsMetadata"] != null
            ? keycloakConfiguration["RequireHttpsMetadata"]!.Equals("true", StringComparison.OrdinalIgnoreCase)
            : true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

var assembly = Assembly.Load("Chatter.Users.Application");
builder.Services.AddCqrs(assembly);
builder.Services.Configure<AesEncryptorOptions>(builder.Configuration.GetSection(AesEncryptorOptions.SectionName));
builder.Services.AddInfrastructure();
builder.Services.AddSharedDataAccessTypes();
builder.Services.AddUsersDataAccess(builder.Configuration);
builder.Services.AddKeycloakService();
builder.Services.AddAppLogger();
builder.Services.Configure<KeycloakConfig>(keycloakConfiguration);
builder.Services.AddValidatorsFromAssembly(assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();