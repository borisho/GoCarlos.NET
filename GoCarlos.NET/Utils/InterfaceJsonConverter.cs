using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoCarlos.NET.Utils;

public class InterfaceJsonConverter<Implementation, Interface> : JsonConverter<Interface>
    where Implementation : class, Interface
{
    public override Interface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<Implementation>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Interface value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
