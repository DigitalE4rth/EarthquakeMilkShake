using System.Globalization;

namespace EarthquakeMilkShake.Facilities;

public class FacilitySettings
{
    public string Location { get; set; } = string.Empty;
    public string Delimiter = ",";
    public string Subfolder = "Data";
    public (int Min, int Max) Years = (1970, 2024);
    public (double Min, double Max) Magnitude = (1, 10);
    public NumberFormatInfo NumberFormatInfo { get; set; } = new() { NumberDecimalSeparator = "." };

    public string MergedFileName { get; set; } = "Merged.csv";
    public string MergedFilePath => Path.Combine(Location, MergedFileName);

    public string ParsedFileName { get; set; } = "Parsed.csv";
    public string ParsedFilePath => Path.Combine(Location, ParsedFileName);

    public string FilteredFileName { get; set; } = "Filtered.csv";
    public string FilteredFilePath => Path.Combine(Location, FilteredFileName);

    public string CountParsedFileName { get; set; } = "CountedParsed.csv";
    public string CountParsedFilePath => Path.Combine(Location, CountParsedFileName);

    public string CountResultFileName { get; set; } = "CountedFiltered.csv";
    public string CountResultFilePath => Path.Combine(Location, CountResultFileName);

    public string CountByMagFilteredFileName { get; set; } = "CountedByMagFiltered.csv";
    public string CountByMagFilteredFilePath => Path.Combine(Location, CountByMagFilteredFileName);

    public string CountByMagParsedFileName { get; set; } = "CountedByMagParsed.csv";
    public string CountByMagParsedFilePath => Path.Combine(Location, CountByMagParsedFileName);

    public string LocationCountAllFileName { get; set; } = "CountByLocationAll.csv";
    public string LocationCountAllFilePath => Path.Combine(Location, LocationCountAllFileName);

    public string LocationCountPartialFileName { get; set; } = "CountByLocationPartial.csv";
    public string LocationCountPartialFilePath => Path.Combine(Location, LocationCountPartialFileName);
}
