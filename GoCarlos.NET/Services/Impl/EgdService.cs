using GoCarlos.NET.Models;
using GoCarlos.NET.Services.Api;
using System;
using System.Net.Http;
using System.Text.Json;

namespace GoCarlos.NET.Services.Impl;

/// <inheritdoc cref="IEgdService"/>
public sealed class EgdService : IEgdService
{
    private static readonly HttpClient client;

    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    static EgdService()
    {
        SocketsHttpHandler socketsHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromSeconds(90)
        };

        client = new HttpClient(socketsHandler);
    }

    public EgdData? SearchByPin(string pin)
    {
        string url = "https://www.europeangodatabase.eu/EGD/GetPlayerDataByPIN.php?pin=" + pin.Trim();

        return JsonSerializer.Deserialize<EgdData>(GetJson(url), options);
    }

    public EgdDataList? SearchByData(string lastName, string name)
    {
        string url = $"https://www.europeangodatabase.eu/EGD/GetPlayerDataByData.php?lastname={lastName.Trim()}&name={name.Trim()}";

        return JsonSerializer.Deserialize<EgdDataList>(GetJson(url), options);
    }

    private static string GetJson(string url)
    {
        using HttpResponseMessage response = client.GetAsync(url).Result;
        using HttpContent content = response.Content;

        string json = content.ReadAsStringAsync().Result;

        return json;
    }
}
