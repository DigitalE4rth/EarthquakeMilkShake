using CsvHelper.Configuration;
using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.CsvToObjMappers;

public sealed class IrisObjMap : ClassMap<EarthquakeRawInfo>
{
    public IrisObjMap()
    {
        Map(m => m.Place).Name("Region");
        Map(m => m.Date).Name("Timestamp");
        Map(m => m.Magnitude).Name("Mag");
        Map(m => m.Depth).Name("Depth");
    }
}
