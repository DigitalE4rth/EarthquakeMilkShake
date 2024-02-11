namespace EarthquakeMilkShake.EarthquakeCount;

public class EqDateDetailedCount
{
    public DateTime Date  { get; set; }
    public int      Year  { get; set; }
    public int      Month { get; set; }
    public int      Day   { get; set; }
    public int      Count { get; set; }

    public EqDateDetailedCount(DateTime date, int year, int month, int day, int count)
    {
        Date  = date;
        Year  = year;
        Month = month;
        Day   = day;
        Count = count;
    }

    public EqDateDetailedCount()
    {
    }
}
