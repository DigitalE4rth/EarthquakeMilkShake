using CsvHelper.Configuration;
using EarthquakeMilkShake.EarthquakeCount;

namespace EarthquakeMilkShake.CsvToObjMappers;

public class EarthQuakesCountMap : ClassMap<EqByYearsCount>
{
    public EarthQuakesCountMap()
    {
        Map(m => m.Year).Name("Year");
        Map(m => m.Count).Name("Count");
    }
}
