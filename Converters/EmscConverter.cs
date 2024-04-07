using EarthquakeMilkShake.DataRaw;
using EarthquakeMilkShake.Utils;

namespace EarthquakeMilkShake.Converters;

public class EmscConverter : IEarthquakeInfoObjConverter
{
    private readonly MiscParsers _parsers = new();

    public List<EarthquakeInfo> ConvertToObj(List<EarthquakeRawInfo> rawData)
    {

        return rawData.Select(raw => new EarthquakeInfo()
        {
            Id = raw.Id,
            Date = _parsers.ParseDateWithTime(raw.Date),
            Magnitude = _parsers.ParseDouble(raw.Magnitude),
            Latitude = _parsers.ParseDouble(raw.Latitude),
            Longitude = _parsers.ParseDouble(raw.Longitude),
            Place = raw.Place,
            Depth = _parsers.ParseDouble(raw.Depth),
            Contributor = raw.Contributor
        }).ToList();
    }
}
