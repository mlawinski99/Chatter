namespace Chatter.SyncUsersJob.Models;

public class KeycloakConfig
{
    public string AuthServerUrl { get; set; }
    public string Realm { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public List<string> ConnectionStrings { get; set; } = new();
}