using Hangfire;
using System.Net.Http.Headers;
using System.Text.Json;
using Chatter.MessagesDomain;
using Chatter.Shared.Encryption.JsonSerializable;
using Chatter.Shared.Logger;
using Chatter.SyncUsersJob.Models;
using Microsoft.EntityFrameworkCore;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chatter.SyncUsersJob;

public class KeycloakEventSyncService
{
    private readonly IAppLogger<KeycloakEventSyncService> _logger;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly HttpClient _httpClient;
    private readonly KeycloakConfig _config;
    private readonly string _requestUrl;
    private readonly string _tokenUrl;
    
    public KeycloakEventSyncService(IHttpClientFactory httpClientFactory, 
        KeycloakConfig config,
        IAppLogger<KeycloakEventSyncService> logger,
        IJsonSerializer jsonSerializer)
    {
        _httpClient = httpClientFactory.CreateClient();
        _config = config;
        _logger = logger;
        _jsonSerializer = jsonSerializer;

        _requestUrl = $"{_config.AuthServerUrl}/admin/realms/{_config.Realm}/admin-events" +
                      "?resourceTypes=USER&operationTypes=CREATE&operationTypes=UPDATE&operationTypes=DELETE";
        _tokenUrl = $"{_config.AuthServerUrl}/realms/{_config.Realm}/protocol/openid-connect/token";
    }

    [DisableConcurrentExecution(60)] 
    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SyncUserEventsAsync()
    {
        _logger.LogInformation("SyncJob started at: {Time} UTC", DateTimeOffset.UtcNow);
        string token;
        try
        {
            _logger.LogInformation("Obtaining Keycloak token...");
            token = await GetToken();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to obtain Keycloak token.", ex);
            throw;
        }

        foreach (var connectionString in _config.ConnectionStrings)
        {
            try
            {
                _logger.LogInformation("Starting synchronization for database: {Database}", 
                    new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Database);
                
                var options = new DbContextOptionsBuilder<SyncDbContext>();
                options.UseNpgsql(connectionString);

                await using var dbContext = new SyncDbContext(options.Options);

                var keycloakLastSync = await dbContext.ConfigurationData
                    .FirstOrDefaultAsync(c => c.Key == KeycloakSyncStaticSettings.SyncJobKeyValue);

                _logger.LogInformation("Fetched last sync time: {LastSync}", keycloakLastSync?.Value ?? "null");
                
                var url = _requestUrl;
                if (!string.IsNullOrEmpty(keycloakLastSync?.Value))
                {
                    // @TODO keycloak 27 - Epoch timestamp millis
                    var parsed = DateTime.Parse(keycloakLastSync.Value);
                    var dateIso = parsed.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                    url += $"&dateFrom={Uri.EscapeDataString(dateIso)}";
                }

                _logger.LogInformation("Preparing request to Keycloak...");
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                
                var events = _jsonSerializer.Deserialize<List<KeycloakAdminEventDto>>(json);
                _logger.LogInformation("Events synced successfully...");
                if (events is { Count: > 0 })
                {
                    // @TODO keyclock 27 - if filter will work in query remove this filter
                    DateTime? parsed = keycloakLastSync is not null
                        ? DateTime.Parse(keycloakLastSync.Value).ToUniversalTime()
                        : null;

                    events.RemoveAll(e =>
                        (parsed.HasValue && e.Time < new DateTimeOffset(parsed.Value).ToUnixTimeMilliseconds()) ||
                        e.ResourceType != "USER" ||
                        (e.OperationType != "CREATE" &&
                         e.OperationType != "UPDATE" &&
                         e.OperationType != "DELETE"));
                }
                else
                {
                    events = new List<KeycloakAdminEventDto>();
                }
                
                if (events.Count == 0)
                {
                    _logger.LogInformation("No events found for database: {Database}",
                        new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Database);
                    return;
                }
                
                var eventsToInsert = events
                    .Select(x => x.ToKeyclockEvent);
                dbContext.AddRange(eventsToInsert);

                var maxEventTime = events.Max(e =>
                    DateTimeOffset.FromUnixTimeMilliseconds(e.Time).UtcDateTime.AddMilliseconds(1));
                if (keycloakLastSync is null)
                {
                    keycloakLastSync = new ConfigurationData { Key = KeycloakSyncStaticSettings.SyncJobKeyValue };
                    dbContext.Add(keycloakLastSync);
                }
                keycloakLastSync.Value = maxEventTime.ToString("o");

                await dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Event synchronization completed for database: {Database}",
                    new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Database);
            }
            catch (Exception ex)
            {
                var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
                _logger.LogError("Error during event synchronization for: {Database}", builder.Database, ex);
            }
        }
    }
    
     private async Task<string> GetToken()
     {
         var response = await _httpClient.PostAsync(
             _tokenUrl,
             new FormUrlEncodedContent(new Dictionary<string, string>
             {
                 {"client_id", _config.ClientId},
                 {"client_secret", _config.ClientSecret},
                 {"grant_type", "client_credentials"}
             }));

         response.EnsureSuccessStatusCode();
         var json = await response.Content.ReadAsStringAsync();
         var token = JsonSerializer.Deserialize<JsonElement>(json).GetProperty("access_token").GetString();
         return token;
     }
}
