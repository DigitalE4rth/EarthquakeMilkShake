namespace EarthquakeMilkShake.DataRaw;

public class EarthquakeRawInfo
{
    public string Id          { get; set; } = string.Empty;
    public string Date        { get; set; } = string.Empty;
    public string Place       { get; set; } = string.Empty;
    public string Magnitude   { get; set; } = string.Empty;
    public string Latitude    { get; set; } = string.Empty;
    public string Longitude   { get; set; } = string.Empty;
    public string Depth       { get; set; } = string.Empty;
    public string Type        { get; set; } = string.Empty;
    public string Status      { get; set; } = string.Empty;
    public string Contributor { get; set; } = string.Empty;

    public EarthquakeRawInfo()
    {
    }
}
