using Microsoft.VisualBasic;
using System.Diagnostics;

namespace EarthquakeMilkShake.Utils;

public class DownloadManager : IDisposable
{
    public readonly HttpClient HttpClient;
    public uint Retries { get; set; } = 50;
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    public DownloadManager()
    {
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
        HttpClient.Timeout = TimeSpan.FromMinutes(15);
    }

    public async Task DownloadBytes(string url, string filePath) => await DownloadBytes(url, filePath, Delay);

    public async Task DownloadBytes(string url, string filePath, TimeSpan delay)
    {
        for (var i = 0; i < Retries; i++)
        {
            byte[]? data;

            try
            {
                data = await HttpClient.GetByteArrayAsync(url);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine($"Could not download data. {i+1}/{Retries} Retry after {delay.TotalMilliseconds} milliseconds");
                await Task.Delay(delay);
                continue;
            }

            await File.WriteAllBytesAsync(filePath, data);
            return;
        }

        Debug.WriteLine($"Could not download data after ({Retries}) retries");
    }


    public async Task Download(DownloadSettings settings)
    {
        if (settings.EndIndex + 1 > settings.UrlList.Count) return;
        if (settings.EndIndex == -1) settings.EndIndex = settings.UrlList.Count - 1;

        if (settings.Delay == TimeSpan.Zero)
        {
            await DownloadWithoutDelay(settings);
            return;
        }

        await DownloadWithDelay(settings);
    }

    public async Task DownloadWithDelay(DownloadSettings settings)
    {
        if (settings.EndIndex+1 > settings.UrlList.Count) return;
        if (settings.EndIndex == -1) settings.EndIndex = settings.UrlList.Count-1;

        for (var i = settings.StartIndex; i <= settings.EndIndex; i++)
        {
            Debug.WriteLine($"Download Status [{i+1}/{settings.UrlList.Count}]{Environment.NewLine}{settings.UrlList[i]}");
            await DownloadBytes(settings.UrlList[i], $"{Path.Combine(settings.BaseDirectory, i.ToString())}_{settings.FileEndingName}.csv", settings.Delay);
            Debug.WriteLine($"Waiting for {settings.Delay.TotalMilliseconds} milliseconds...");
            await Task.Delay(settings.Delay);
        }
    }

    public async Task DownloadWithoutDelay(DownloadSettings settings)
    {
        if (settings.EndIndex + 1 > settings.UrlList.Count) return;
        if (settings.EndIndex == -1) settings.EndIndex = settings.UrlList.Count - 1;

        for (var i = settings.StartIndex; i <= settings.EndIndex; i++)
        {
            Debug.WriteLine($"Download Status [{i + 1}/{settings.UrlList.Count}]{Environment.NewLine}{settings.UrlList[i]}");
            await DownloadBytes(settings.UrlList[i], $"{Path.Combine(settings.BaseDirectory, i.ToString())}_{settings.FileEndingName}.csv");
        }
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}
