using System.Globalization;

namespace EarthquakeMilkShake.Utils;

public class MiscParsers
{
    public double ParseDouble(string rawNumber)
    {
        if (!double.TryParse(rawNumber, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return 0;
        return result;
    }

    public DateTime ParseDateWithTime(string rawDate)
    {
        int.TryParse(rawDate.Substring(0, 4),  out var year);
        int.TryParse(rawDate.Substring(5, 2),  out var month);
        int.TryParse(rawDate.Substring(8, 2),  out var day);
        int.TryParse(rawDate.Substring(11, 2), out var hour);
        int.TryParse(rawDate.Substring(14, 2), out var minute);
        int.TryParse(rawDate.Substring(17, 2), out var second);

        return new DateTime(year, month, day, hour, minute, second);
        
    }

    public DateTime ParseDate(string rawDate)
    {
        int.TryParse(rawDate.Substring(0, 4), out var year);
        int.TryParse(rawDate.Substring(5, 2), out var month);
        int.TryParse(rawDate.Substring(8, 2), out var day);

        return new DateTime(year, month, day);
    }
}
