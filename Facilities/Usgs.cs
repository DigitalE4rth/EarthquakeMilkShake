using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.EarthquakeCount;

namespace EarthquakeMilkShake.Facilities;

public class Usgs : FacilityBase<UsgsObjMap, UsgsConverter>
{
    protected string LocationCountAllFileName { get; set; } = "CountByLocationAll.csv";
    protected string LocationCountAllFilePath => Path.Combine(Location, LocationCountAllFileName);

    protected string LocationCountPartialFileName { get; set; } = "CountByLocationPartial.csv";
    protected string LocationCountPartialFilePath => Path.Combine(Location, LocationCountPartialFileName);



    #region Download
    public override List<string> GetDownloadLinks(int yearMin, int yearMax, int magnitudeMin, int magnitudeMax)
    {
        var result = new List<string>
        {
            $"https://earthquake.usgs.gov/fdsnws/event/1/query.csv?starttime={yearMin}-01-01%2000:00:00&endtime={yearMin}-01-01%2000:00:00&minmagnitude={magnitudeMin}&maxmagnitude={magnitudeMax}&eventtype=earthquake&contributor=us&orderby=time-asc"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add($"https://earthquake.usgs.gov/fdsnws/event/1/query.csv?starttime={i}-01-01%2000:00:01&endtime={i + 1}-01-01%2000:00:00&minmagnitude={magnitudeMin}&maxmagnitude={magnitudeMax}&eventtype=earthquake&contributor=us&orderby=time-asc");
        }

        return result;
    }
    #endregion



    #region Create
    public override void FilterAndSave()
    {
        var data = GetParsed()
            .Where(i => i.Contributor.Equals("us"))
            .ToList();

        DeleteResult();
        Save(data, FilteredFilePath);
    }

    public void SaveCountByLocationAll() => SaveCountByLocation(Years.Min, Years.Max, LocationCountAllFileName);
    public void SaveCountByLocationPartial() => SaveCountByLocation(2000, 2024, LocationCountPartialFileName);

    public void SaveCountByLocation(int minYear, int maxYear, string fileName)
    {
        var data = GetEqByLocation(minYear, maxYear);
        Save(data, Path.Combine(Location, fileName));
    }

    protected void Save(List<EqLocationCount> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Delimiter });
        csv.WriteRecords(data);
    }
    #endregion



    #region Read
    protected override List<string> GetWorkFilePaths()
    {
        var baseFiles = base.GetWorkFilePaths();
        baseFiles.Add(LocationCountAllFilePath);
        baseFiles.Add(LocationCountPartialFilePath);
        return baseFiles;
    }

    public List<EqLocationCount> GetEqByLocation(int minYear, int maxYear)
    {
        return GetResult()
            .Where(i => i.Date.Year >= minYear && i.Date.Year <= maxYear)
            .GroupBy(i => AdjustLocationName(i.Place))
            .ToDictionary(i => i.Key, i => i.ToList())
            .Select(i => new EqLocationCount(i.Key, i.Value.Count))
            .OrderByDescending(o => o.Count)
            .ToList();
    }

    private string AdjustLocationName(string location)
    {
        location = location.ToLower();

        var countryIndex = location.LastIndexOf(", ", StringComparison.Ordinal);
        if (countryIndex != -1)
        {
            location = location.Substring(countryIndex + 2);
        }

        location = location.Replace(" region", "")
            .Replace("north of the ", "")
            .Replace("south of the ", "")
            .Replace("east of the ", "")
            .Replace("west of the ", "")
            .Replace("northern ", "")
            .Replace("southern ", "")
            .Replace("western ", "")
            .Replace("eastern ", "")
            .Replace("central ", "")
            .Replace("north of ", "")
            .Replace("south of ", "")
            .Replace("east of ", "")
            .Replace("west of ", "")
            .Replace("off the coast of ", "")
            .Replace("off the north coast of ", "")
            .Replace("off the south coast of ", "")
            .Replace("off the east coast of ", "")
            .Replace("off the west coast of ", "")
            .Replace("north island of ", "")
            .Replace("south island of ", "")
            .Replace("east island of ", "")
            .Replace("west island of ", "")
            .Replace("northeast ", "")
            .Replace("northwest ", "")
            .Replace("southeast ", "")
            .Replace("southwest ", "")
            .Replace("near the coast of ", "")

            .Replace("fiji islands", "fiji")

            .Replace("alabama", "usa")
            .Replace("arizona", "usa")
            .Replace("arkansas", "usa")
            .Replace("california", "usa")
            .Replace("colorado", "usa")
            .Replace("connecticut", "usa")
            .Replace("delaware", "usa")
            .Replace("florida", "usa")
            .Replace("georgia", "usa")
            .Replace("hawaii", "usa")
            .Replace("idaho", "usa")
            .Replace("illinois", "usa")
            .Replace("indiana", "usa")
            .Replace("iowa", "usa")
            .Replace("kansas", "usa")
            .Replace("kentucky", "usa")
            .Replace("louisiana", "usa")
            .Replace("maine", "usa")
            .Replace("maryland", "usa")
            .Replace("massachusetts", "usa")
            .Replace("michigan", "usa")
            .Replace("minnesota", "usa")
            .Replace("mississippi", "usa")
            .Replace("missouri", "usa")
            .Replace("montana", "usa")
            .Replace("nebraska", "usa")
            .Replace("nevada", "usa")
            .Replace("new hampshire", "usa")
            .Replace("new jersey", "usa")
            .Replace("new mexico", "usa")
            .Replace("new york", "usa")
            .Replace("north carolina", "usa")
            .Replace("north dakota", "usa")
            .Replace("ohio", "usa")
            .Replace("oklahoma", "usa")
            .Replace("oregon", "usa")
            .Replace("pennsylvania", "usa")
            .Replace("rhode Island", "usa")
            .Replace("south Carolina", "usa")
            .Replace("south Dakota", "usa")
            .Replace("tennessee", "usa")
            .Replace("texas", "usa")
            .Replace("utah", "usa")
            .Replace("vermont", "usa")
            .Replace("virginia", "usa")
            .Replace("washington", "usa")
            .Replace("west Virginia", "usa")
            .Replace("wisconsin", "usa")
            .Replace("wyoming", "usa");

        return location;
    }
    #endregion



    #region Delete
    public void DeleteLocationCountAll() => File.Delete(LocationCountAllFilePath);
    public void DeleteLocationCountPartial() => File.Delete(LocationCountPartialFilePath);
    public void DeleteLocationCount()
    {
        DeleteLocationCountAll();
        DeleteLocationCountPartial();
    }
    public override void DeleteWorkFiles()
    {
        DeleteLocationCount();
        base.DeleteWorkFiles();
    }
    #endregion
}
