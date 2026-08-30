using Newtonsoft.Json;
using System;

namespace GoCarlos.NET.Models.Converters;

public class DecimalConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(decimal);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;

        if (!decimal.TryParse(reader.Value.ToString(), out var value))
            throw new JsonSerializationException($"Unable to parse '{reader.Value}' as decimal.");

        return value;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
