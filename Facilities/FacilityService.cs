using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EarthquakeMilkShake.EarthquakeCount;

namespace EarthquakeMilkShake.Facilities;

public class FacilityService
{
    private readonly Emsc _emsc;
    private readonly Iris _iris;
    private readonly Isc  _isc;
    private readonly Usgs _usgs;

    private readonly string _delimiter = ",";
    private string Location { get; set; }
    private string TotalRawCountFileName { get; set; } = "GlobalCount.csv";
    private string TotalRawCountFilePath => Path.Combine(Location, TotalRawCountFileName);
    private string TotalResultCountFileName { get; set; } = "GlobalFilteredCount.csv";
    private string TotalResultCountFilePath => Path.Combine(Location, TotalResultCountFileName);

    public FacilityService(Emsc emsc, Iris iris, Isc isc, Usgs usgs)
    {
        _emsc = emsc;
        _iris = iris;
        _isc  = isc;
        _usgs = usgs;

        Location = Path.Combine(AppContext.BaseDirectory, "Data", "Global");
        Directory.CreateDirectory(Location);
    }

    #region Create
    public void CountCombinedParsedAndSave()
    {
        var data = GetCombinedRawData();
        Save(data, TotalRawCountFilePath);
    }
    public void CountCombinedFilteredAndSave()
    {
        var data = GetCombinedResultData();
        Save(data, TotalResultCountFilePath);
    }
    private void Save(List<EqFacilitiesCount> data, string filePath)
    {
        File.Delete(filePath);
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = _delimiter });
        csv.WriteRecords(data);
    }
    private List<EqFacilitiesCount> PopulateData(int minYear, int maxYear)
    {
        var result = new List<EqFacilitiesCount>();
        for (var i = minYear; i <= maxYear; i++)
        {
            result.Add(new EqFacilitiesCount { Year = i });
        }
        return result;
    }
    private List<EqFacilitiesCount> CombineData(Dictionary<int, EqByYearsCount> esmcDict,
                                                Dictionary<int, EqByYearsCount> irisDict,
                                                Dictionary<int, EqByYearsCount> iscDict,
                                                Dictionary<int, EqByYearsCount> usgsDict)
    {
        var result = PopulateData(1970, 2024).ToDictionary(r => r.Year);
        foreach (var year in result.Keys)
        {
            esmcDict.TryGetValue(year, out var esmcCount);
            result[year].Emsc = esmcCount?.Count ?? 0;

            irisDict.TryGetValue(year, out var irisCount);
            result[year].Iris = irisCount?.Count ?? 0;

            iscDict.TryGetValue(year, out var iscCount);
            result[year].Isc = iscCount?.Count ?? 0;

            usgsDict.TryGetValue(year, out var usgsCount);
            result[year].Usgs = usgsCount?.Count ?? 0;
        }

        return result.Values.ToList();
    }
    #endregion



    #region Read
    public List<EqFacilitiesCount> GetCombinedRawData()
    {
        var esmcDict = _emsc.GetCountParsed().ToDictionary(e => e.Year);
        var irisDict = _iris.GetCountParsed().ToDictionary(e => e.Year);
        var iscDict  = _isc.GetCountParsed().ToDictionary(e => e.Year);
        var usgsDict = _usgs.GetCountParsed().ToDictionary(e => e.Year);

        return CombineData(esmcDict, irisDict, iscDict, usgsDict);
    }
    public List<EqFacilitiesCount> GetCombinedResultData()
    {
        var esmcDict = _emsc.GetCountResult().ToDictionary(e => e.Year);
        var irisDict = _iris.GetCountResult().ToDictionary(e => e.Year);
        var iscDict = _isc.GetCountResult().ToDictionary(e => e.Year);
        var usgsDict = _usgs.GetCountResult().ToDictionary(e => e.Year);

        return CombineData(esmcDict, irisDict, iscDict, usgsDict);
    }
    #endregion



    #region Delete
    public void DeleteRawCount() => File.Delete(TotalRawCountFilePath);
    public void DeleteResultCount() => File.Delete(TotalResultCountFilePath);
    public void DeleteAll()
    {
        DeleteRawCount();
        DeleteResultCount();
    }
    #endregion
}
