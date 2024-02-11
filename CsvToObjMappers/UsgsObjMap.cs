using CsvHelper.Configuration;
using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.CsvToObjMappers;

public sealed class UsgsObjMap : ClassMap<EarthquakeRawInfo>
{
    public UsgsObjMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Date).Name("time");
        Map(m => m.Place).Name("place");
        Map(m => m.Magnitude).Name("mag");
        Map(m => m.Depth).Name("depth");
        Map(m => m.Type).Name("type");
        Map(m => m.Status).Name("status");
        Map(m => m.Contributor).Name("locationSource");
    }
}
