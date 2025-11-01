using Chatter.Shared.Encryption;
using Chatter.Shared.Logger;
using Chatter.SyncUsersJob;
using Chatter.SyncUsersJob.Models;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var hangfireConnection = builder.Configuration.GetConnectionString("HangfireDb");

builder.Services.AddDbContext<HangfireDbContext>(options =>
    options.UseNpgsql(hangfireConnection));

using var tempProvider = builder.Services.BuildServiceProvider();
var dbContext = tempProvider.GetRequiredService<HangfireDbContext>();
Console.WriteLine("Verifying Hangfire database exists...");
await dbContext.Database.EnsureCreatedAsync();
Console.WriteLine("Verification succeeded");

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(hangfireConnection));

builder.Services.AddHangfireServer();

builder.Services.AddHttpClient();
var keycloakConfig = new KeycloakConfig();
builder.Configuration.GetSection("Keycloak").Bind(keycloakConfig);
builder.Services.AddSingleton(keycloakConfig);
builder.Services.AddSingleton<KeycloakEventSyncService>();
builder.Services.AddAppLogger();
builder.Services.AddInfrastructure();
var app = builder.Build();

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<KeycloakEventSyncService>(
    "keycloak-user-sync-job",
    job => job.SyncUserEventsAsync(),
    "*/30 * * * * *",  
    TimeZoneInfo.Utc
);

await app.RunAsync();