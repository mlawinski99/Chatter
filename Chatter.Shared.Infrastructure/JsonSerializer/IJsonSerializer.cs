namespace Chatter.Shared.Encryption.JsonSerializable;

public interface IJsonSerializer
{
    T Deserialize<T>(string value);
    string Serialize<T>(T value);
}