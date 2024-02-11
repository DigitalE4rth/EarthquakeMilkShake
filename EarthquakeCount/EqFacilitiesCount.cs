namespace EarthquakeMilkShake.EarthquakeCount;

public class EqFacilitiesCount
{
    public int Year { get; set; }
    public int Emsc { get; set; }
    public int Iris { get; set; }
    public int Isc  { get; set; }
    public int Usgs { get; set; }

    public EqFacilitiesCount(int year, int emscCount, int irisCount, int iscCount, int usgsCount)
    {
        Year = year;
        Emsc = emscCount;
        Iris = irisCount;
        Isc  = iscCount;
        Usgs = usgsCount;
    }

    public EqFacilitiesCount()
    {
    }
}
