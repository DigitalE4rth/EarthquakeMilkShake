using CsvHelper.Configuration;
using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.CsvToObjMappers;

public sealed class IrisWilberObjMap : ClassMap<EarthquakeRawInfo>
{
    public IrisWilberObjMap()
    {
        Map(m => m.Id).Name("#EventID");
        Map(m => m.Date).Name("Time");
        Map(m => m.Place).Name("EventLocationName");
        Map(m => m.Magnitude).Name("Magnitude");
        Map(m => m.Depth).Name("Depth/km");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
        Map(m => m.Type).Name("MagType");
        Map(m => m.Contributor).Name("Contributor");
    }
}
