using CsvHelper.Configuration;
using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.CsvToObjMappers;

public sealed class IrisIebObjMap : ClassMap<EarthquakeRawInfo>
{
    public IrisIebObjMap()
    {
        Map(m => m.Place).Name("Region");
        Map(m => m.Date).Name("Timestamp");
        Map(m => m.Magnitude).Name("Mag");
        Map(m => m.Latitude).Name("Lat");
        Map(m => m.Longitude).Name("Lon");
        Map(m => m.Depth).Name("Depth");
    }
}
