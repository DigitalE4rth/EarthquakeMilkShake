namespace EarthquakeMilkShake.DataRaw;

public class EarthquakeInfo
{
    public string   Id          { get; set; } = string.Empty;
    public DateTime Date        { get; set; }
    public string   Place       { get; set; } = string.Empty;
    public double   Magnitude   { get; set; }
    public double   Depth       { get; set; }
    public string   Type        { get; set; } = string.Empty;
    public string   Status      { get; set; } = string.Empty;
    public string   Contributor { get; set; } = string.Empty;

    public EarthquakeInfo(string   id,
                          DateTime date,
                          string   place,
                          double   magnitude,
                          double   depth,
                          string   type,
                          string   status,
                          string   contributor)
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

    public EarthquakeInfo()
    {
    }
}
