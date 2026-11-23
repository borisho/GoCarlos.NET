// Copyright (C) 2023 Boris Dovčík
// This file is part of GoCarlos.NET.
//
// GoCarlos.NET is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 3, or (at your option) any
// later version.
//
// GoCarlos.NET is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with GoCarlos.NET. If not, see <https://www.gnu.org/licenses/>.

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
