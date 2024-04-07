namespace EarthquakeMilkShake.Utils;

public class DownloadSettings
{
    public List<string> UrlList { get; set; } = new(0);
    public string BaseDirectory { get; set; } = string.Empty;
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;
    public string FileEndingName { get; set; } = string.Empty;
    public int StartIndex { get; set; }
    public int EndIndex { get; set; } = -1;
}
