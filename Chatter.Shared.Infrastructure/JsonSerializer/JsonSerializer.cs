using Newtonsoft.Json;

namespace Chatter.Shared.Encryption.JsonSerializable;

public class JsonSerializer : IJsonSerializer
{
    public T Deserialize<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>(value);
    }

    public string Serialize<T>(T value)
    {
        return JsonConvert.SerializeObject(value);
    }
}