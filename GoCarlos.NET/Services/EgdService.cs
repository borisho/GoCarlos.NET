using GoCarlos.NET.Interfaces;
using GoCarlos.NET.Models;
using GoCarlos.NET.Utils;
using System;
using System.Net.Http;
using System.Text.Json;

namespace GoCarlos.NET.Services;

/// <inheritdoc cref="IEgdService"/>
public sealed class EgdService : IEgdService
{
    private static readonly HttpClient client;

    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = {
            new InterfaceJsonConverter<EgdDataList, IEgdDataList>(),
            new InterfaceJsonConverter<EgdData, IEgdData>(),
        },
    };

    static EgdService()
    {
        SocketsHttpHandler socketsHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromSeconds(90)
        };

        client = new HttpClient(socketsHandler);
    }

    public IEgdData? SearchByPin(string pin)
    {
        string url = "https://www.europeangodatabase.eu/EGD/GetPlayerDataByPIN.php?pin=" + pin.Trim();

        return JsonSerializer.Deserialize<IEgdData>(GetJson(url), options);
    }

    public IEgdDataList? SearchByData(string lastName, string name)
    {
        string url = $"https://www.europeangodatabase.eu/EGD/GetPlayerDataByData.php?lastname={lastName.Trim()}&name={name.Trim()}";

        return JsonSerializer.Deserialize<IEgdDataList>(GetJson(url), options);
    }

    private static string GetJson(string url)
    {
        using HttpResponseMessage response = client.GetAsync(url).Result;
        using HttpContent content = response.Content;

        string json = content.ReadAsStringAsync().Result;

        return json;
    }
}
