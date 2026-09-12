using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Models.Converters;

public class CriteriaConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ObservableCollection<Criteria>);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jArray = JArray.Load(reader);
        var collection = new ObservableCollection<Criteria>();

        foreach (var item in jArray)
        {
            var criteria = item.ToObject<Criteria>(new JsonSerializer());

            if (criteria != null) collection.Add(criteria);
        }

        return collection;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var collection = value as ObservableCollection<Criteria>;
        serializer.Serialize(writer, collection);
    }
}
