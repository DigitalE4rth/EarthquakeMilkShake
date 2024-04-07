using CsvHelper.Configuration;
using EarthquakeMilkShake.DataRaw;

namespace EarthquakeMilkShake.CsvToObjMappers;

public sealed class IscObjMap : ClassMap<EarthquakeRawInfo>
{
    public IscObjMap()
    {
        Map(m => m.Id).Name("EVENTID");
        Map(m => m.Date).Name("DATE");
        Map(m => m.Magnitude).Name("MAG");
        Map(m => m.Depth).Name("DEPTH");
        Map(m => m.Latitude).Name("LAT");
        Map(m => m.Longitude).Name("LON");
        Map(m => m.Type).Name("TYPE");
        Map(m => m.Contributor).Name("AUTHOR");
    }
}
