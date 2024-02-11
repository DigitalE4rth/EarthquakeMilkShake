using EarthquakeMilkShake.DataRaw;
using EarthquakeMilkShake.Utils;

namespace EarthquakeMilkShake.Converters;

public class UsgsConverter : IEarthquakeInfoObjConverter
{
    private readonly MiscParsers _parsers = new();

    public List<EarthquakeInfo> ConvertToObj(List<EarthquakeRawInfo> rawData)
    {
        return rawData.Select(raw => new EarthquakeInfo()
        {
            Id = raw.Id,
            Date = _parsers.ParseDateWithTime(raw.Date),
            Magnitude = _parsers.ParseDouble(raw.Magnitude),
            Place = raw.Place,
            Depth = _parsers.ParseDouble(raw.Depth),
            Contributor = raw.Contributor,
            Type = raw.Type,
            Status = "reviewed"
        }).ToList();
    }
}
