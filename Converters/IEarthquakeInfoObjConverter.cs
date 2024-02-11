using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.Converters;

public interface IEarthquakeInfoObjConverter
{
    public List<EarthquakeInfo> ConvertToObj(List<EarthquakeRawInfo> rawData);
}
