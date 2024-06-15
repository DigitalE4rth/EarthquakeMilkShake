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

    public string CountByLocationFilteredFileName { get; set; } = "CountByLocationFiltered.csv";
    public string CountByLocationFilteredFilePath => Path.Combine(Location, CountByLocationFilteredFileName);

    public string CountByLocationParsedFileName { get; set; } = "CountByLocationParsed.csv";
    public string CountByLocationParsedFilePath => Path.Combine(Location, CountByLocationParsedFileName);

    public string CountByDepthFilteredFileName { get; set; } = "CountByDepthFiltered.csv";
    public string CountByDepthFilteredFilePath => Path.Combine(Location, CountByDepthFilteredFileName);
                         
    public string CountByDepthParsedFileName { get; set; } = "CountByDepthParsed.csv";
    public string CountByDepthParsedFilePath => Path.Combine(Location, CountByDepthParsedFileName);

    public string CountByLocationPartialFileName { get; set; } = "CountByLocationPartial.csv";
    public string CountByLocationPartialFilePath => Path.Combine(Location, CountByLocationPartialFileName);
}
