using System.Net.Http;
using System;
using GoCarlos.NET.Interfaces;

namespace GoCarlos.MAUI.Services;

public sealed class EgdService : IEgdService
{
    private static readonly HttpClient client;

    static EgdService()
    {
        SocketsHttpHandler socketsHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromSeconds(90)
        };

        client = new HttpClient(socketsHandler);
    }

    public string SearchByPin(string pin)
    {
        string url = "https://www.europeangodatabase.eu/EGD/GetPlayerDataByPIN.php?pin=" + pin;
        return GetJson(url);
    }

    public string SearchByData(string lastName, string name)
    {
        string url = $"https://www.europeangodatabase.eu/EGD/GetPlayerDataByData.php?lastname={lastName}&name={name}";
        return GetJson(url);
    }

    private static string GetJson(string url)
    {
        using HttpResponseMessage response = client.GetAsync(url).Result;
        using HttpContent content = response.Content;

        string json = content.ReadAsStringAsync().Result;

        return json;
    }
}
