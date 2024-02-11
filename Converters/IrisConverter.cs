using EarthquakeMilkShake.DataRaw;
using EarthquakeMilkShake.Utils;

namespace EarthquakeMilkShake.Converters;

public class IrisConverter : IEarthquakeInfoObjConverter
{
    private readonly MiscParsers _parsers = new();

    public List<EarthquakeInfo> ConvertToObj(List<EarthquakeRawInfo> rawData)
    {
        return rawData.Select(raw => new EarthquakeInfo()
        {
            Date = FormatDateString(raw.Date),
            Magnitude = _parsers.ParseDouble(raw.Magnitude),
            Place = raw.Place,
            Depth = _parsers.ParseDouble(raw.Depth),
        }).ToList();
    }

    private DateTime FormatDateString(string rawDate)
    {
        if (!int.TryParse(rawDate, out var timestamp)) return DateTime.MinValue;
        var parsedDateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        return new DateTime(parsedDateTime.Year, parsedDateTime.Month, parsedDateTime.Day, parsedDateTime.Hour, parsedDateTime.Minute, parsedDateTime.Second);
    }
}
