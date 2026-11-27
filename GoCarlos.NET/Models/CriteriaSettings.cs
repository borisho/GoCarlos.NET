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

using CommunityToolkit.Mvvm.ComponentModel;
using GoCarlos.NET.Models.Converters;
using GoCarlos.NET.Models.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoCarlos.NET.Models;

[method: JsonConstructor]
public partial class CriteriaSettings() : ObservableObject
{
    private static readonly Dictionary<CriteriaType, Criteria> allCriterias = new()
    {
        { CriteriaType.NUL, new Criteria(CriteriaType.NUL, "Null", "Bez parametra") },
        { CriteriaType.POV, new Criteria(CriteriaType.POV, "Počet výhier", "") },
        { CriteriaType.MMS, new Criteria(CriteriaType.MMS, "McMahon skóre", "") },
        { CriteriaType.RAT, new Criteria(CriteriaType.RAT, "Rating", "Rating od -900 do ∞") },
        { CriteriaType.TRD, new Criteria(CriteriaType.TRD, "Trieda", "Trieda od 9d do 30k") },
        { CriteriaType.SOS, new Criteria(CriteriaType.SOS, "SOS", "Súčet skóre súperov") },
        { CriteriaType.SDS, new Criteria(CriteriaType.SDS, "SODOS", "Súčet skóre porazených súperov") },
        { CriteriaType.SSS, new Criteria(CriteriaType.SSS, "SOSOS", "Súčet SOS súperov") },
        { CriteriaType.VZP, new Criteria(CriteriaType.VZP, "Vzájomná partia", "Víťaz vzájomnej partie berie lepšiu priečku") }
    };

    public CriteriaSettings(IEnumerable<Criteria> criterias) : this()
    {
        Criterias = [.. criterias];
    }

    [ObservableProperty]
    [JsonProperty]
    [JsonConverter(typeof(CriteriaConverter))]
    public partial ObservableCollection<Criteria> Criterias { get; private set; } = [];

    public static Dictionary<CriteriaType, Criteria> AllCriteriaDict => allCriterias;

    public static Criteria[] AllCriteria => [.. allCriterias.Values];
}