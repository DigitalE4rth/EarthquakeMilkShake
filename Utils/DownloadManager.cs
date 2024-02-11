using System.Diagnostics;

namespace EarthquakeMilkShake.Utils;

public class DownloadManager : IDisposable
{
    public readonly HttpClient HttpClient;

    public DownloadManager()
    {
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
        HttpClient.Timeout = TimeSpan.FromMinutes(10);
    }

    public async Task Download(string url, string filePath)
    {
        var bytes = await HttpClient.GetByteArrayAsync(url);
        await File.WriteAllBytesAsync(filePath, bytes);
    }

    public async Task Download(List<string> urls, string baseDirectory, TimeSpan delay)
    {
        for (var i = 0; i < urls.Count; i++)
        {
            Debug.WriteLine($"Download Status [{i+1}/{urls.Count}]{Environment.NewLine}{urls[i]}");
            await Download(urls[i], $"{Path.Combine(baseDirectory, i.ToString())}.csv");
            Debug.WriteLine($"Waiting for {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }

    public async Task Download(List<string> urls, string baseDirectory)
    {
        for (var i = 0; i < urls.Count; i++)
        {
            Debug.WriteLine($"Download Status [{i + 1}/{urls.Count}]{Environment.NewLine}{urls[i]}");
            await Download(urls[i], $"{Path.Combine(baseDirectory, i.ToString())}.csv");
        }
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}
