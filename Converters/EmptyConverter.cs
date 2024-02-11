using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.Converters;

public class EmptyConverter : IEarthquakeInfoObjConverter
{
    public List<EarthquakeInfo> ConvertToObj(List<EarthquakeRawInfo> rawData)
    {
        return new List<EarthquakeInfo>(0);
    }
}
