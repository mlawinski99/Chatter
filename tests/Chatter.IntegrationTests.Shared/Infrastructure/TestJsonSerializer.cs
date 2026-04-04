using Core.Infrastructure.Json;
using Newtonsoft.Json;

namespace Chatter.IntegrationTests.Shared.Infrastructure;

public class TestJsonSerializer : IJsonSerializer
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
