namespace EarthquakeMilkShake.EarthquakeCount;

public class EqByYearsCount
{
    public int Year { get; set; }
    public int Count { get; set; }

    public EqByYearsCount(int year, int count)
    {
        Year  = year;
        Count = count;
    }

    public EqByYearsCount()
    {
    }
}
