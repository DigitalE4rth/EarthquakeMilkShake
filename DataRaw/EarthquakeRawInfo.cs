namespace EarthquakeMilkShake.DataRaw;

public class EarthquakeRawInfo
{
    public string Id          { get; set; } = string.Empty;
    public string Date        { get; set; } = string.Empty;
    public string Place       { get; set; } = string.Empty;
    public string Magnitude   { get; set; } = string.Empty;
    public string Depth       { get; set; } = string.Empty;
    public string Type        { get; set; } = string.Empty;
    public string Status      { get; set; } = string.Empty;
    public string Contributor { get; set; } = string.Empty;

    public EarthquakeRawInfo(string id,
                             string date,
                             string place,
                             string magnitude,
                             string depth,
                             string type,
                             string status,
                             string contributor)
    {
        Id          = id;
        Date        = date;
        Place       = place;
        Magnitude   = magnitude;
        Depth       = depth;
        Type        = type;
        Status      = status;
        Contributor = contributor;
    }

    public EarthquakeRawInfo()
    {
    }
}
