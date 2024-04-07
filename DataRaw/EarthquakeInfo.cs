namespace EarthquakeMilkShake.DataRaw;

public class EarthquakeInfo
{
    public string   Id          { get; set; } = string.Empty;
    public DateTime Date        { get; set; }
    public string   Place       { get; set; } = string.Empty;
    public double   Latitude    { get; set; }
    public double   Longitude   { get; set; }
    public double   Magnitude   { get; set; }
    public double   Depth       { get; set; }
    public string   Type        { get; set; } = string.Empty;
    public string   Status      { get; set; } = string.Empty;
    public string   Contributor { get; set; } = string.Empty;

    public EarthquakeInfo()
    {
    }
}
