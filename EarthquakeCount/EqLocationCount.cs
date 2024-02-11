namespace EarthquakeMilkShake.EarthquakeCount;

public class EqLocationCount
{
    public string Location { get; set; } = string.Empty;
    public int Count       { get; set; }

    public EqLocationCount(string location, int count)
    {
        Location = location;
        Count    = count;
    }

    public EqLocationCount()
    {
    }
}
