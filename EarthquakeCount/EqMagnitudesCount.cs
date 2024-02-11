namespace EarthquakeMilkShake.EarthquakeCount;

public class EqMagnitudesCount
{
    public int Year  { get; set; }
    public int One   { get; set; }
    public int Two   { get; set; }
    public int Three { get; set; }
    public int Four  { get; set; }
    public int Five  { get; set; }
    public int Six   { get; set; }
    public int Seven { get; set; }
    public int Eight { get; set; }
    public int Nine  { get; set; }
    public int Ten   { get; set; }

    public EqMagnitudesCount(int year, int one, int two, int three, int four, int five, int six, int seven, int eight, int nine, int ten)
    {
        Year  = year;
        One   = one;
        Two   = two;
        Three = three;
        Four  = four;
        Five  = five;
        Six   = six;
        Seven = seven;
        Eight = eight;
        Nine  = nine;
        Ten   = ten;
    }

    public EqMagnitudesCount()
    {
    }
}
