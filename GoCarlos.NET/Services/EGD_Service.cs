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

using System;
using System.Diagnostics;
using System.Net.Http;

namespace GoCarlos.NET.Services;

public static class EGDService
{
    private static readonly HttpClient client;

    static EGDService()
    {
        var socketsHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromSeconds(90)
        };

        client = new HttpClient(socketsHandler);
    }

    public static string SearchByPin(string pin)
    {
        string url = "https://www.europeangodatabase.eu/EGD/GetPlayerDataByPIN.php?pin=" + pin;
        return GetJson(url);
    }

    public static string SearchByData(string lastName, string name)
    {
        string url = $"https://www.europeangodatabase.eu/EGD/GetPlayerDataByData.php?lastname={lastName}&name={name}";
        return GetJson(url);
    }

    private static string GetJson(string url)
    {
        Debug.WriteLine(url);

        using HttpResponseMessage response = client.GetAsync(url).Result;
        using HttpContent content = response.Content;

        string json = content.ReadAsStringAsync().Result;

        return json;
    }
}
