using EarthquakeMilkShake.DataRaw;
using EarthquakeMilkShake.Utils;

namespace EarthquakeMilkShake.Converters;

public class IscConverter : IEarthquakeInfoObjConverter
{
    private readonly MiscParsers _parsers = new();

    public List<EarthquakeInfo> ConvertToObj(List<EarthquakeRawInfo> rawData)
    {
        return rawData.Select(raw => new EarthquakeInfo()
        {
            Id = raw.Id,
            Date = _parsers.ParseDate(raw.Date),
            Magnitude = _parsers.ParseDouble(raw.Magnitude),
            Depth = _parsers.ParseDouble(raw.Depth),
            Contributor = raw.Contributor,
            Type = raw.Type
        }).ToList();
    }
}
