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

using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;

namespace GoCarlos.NET.Models;

public class Criteria(CriteriaType type, string name, string desc)
{
    private string abbreviation = type.ToString();

    public CriteriaType Type { get => type; set => type = value; }

    [JsonIgnore]
    public string Abbreviation { get => abbreviation; set => abbreviation = value; }

    [JsonIgnore]
    public string Name { get => name; set => name = value; }

    [JsonIgnore]
    public string Description { get => desc; set => desc = value; }
}