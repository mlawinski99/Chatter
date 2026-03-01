namespace Chatter.Shared.Encryption;

public class AesEncryptorOptions
{
    public const string SectionName = "Encryption";

    public string Key { get; set; }
}