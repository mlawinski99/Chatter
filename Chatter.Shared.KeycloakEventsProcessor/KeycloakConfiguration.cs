namespace Chatter.Shared.UserEventsProcessor;

public class KeycloakConfiguration
{
    public const string SectionName = "Keycloak";

    public string Authority { get; set; } = String.Empty;
    public string Audience { get; set; } = String.Empty;
    public bool RequireHttpsMetadata { get; set; } = false;
}