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
