using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.DataRaw;
using EarthquakeMilkShake.EarthquakeCount;
using EarthquakeMilkShake.Utils;

namespace EarthquakeMilkShake.Facilities;

public abstract class FacilityBase<T, TC> : IFacility where T : ClassMap, new() where TC : IEarthquakeInfoObjConverter, new()
{
    protected string Location { get; set; }

    protected string Delimiter = ",";
    protected string Subfolder = "Data";
    protected (int Min, int Max) Years = (1970, 2023);
    protected (int Min, int Max) Magnitude = (4, 10);

    protected string MergedFileName { get; set; } = "Merged.csv";
    protected string MergedFilePath => Path.Combine(Location, MergedFileName);

    protected string ParsedFileName { get; set; } = "Parsed.csv";
    protected string ParsedFilePath => Path.Combine(Location, ParsedFileName);

    protected string FilteredFileName { get; set; } = "Filtered.csv";
    protected string FilteredFilePath => Path.Combine(Location, FilteredFileName);

    protected string CountParsedFileName  { get; set; } = "CountedParsed.csv";
    protected string CountParsedFilePath => Path.Combine(Location, CountParsedFileName);

    protected string CountResultFileName  { get; set; } = "CountedFiltered.csv";
    protected string CountResultFilePath => Path.Combine(Location, CountResultFileName);

    protected FacilityBase()
    {
        Location = Path.Combine(AppContext.BaseDirectory, Subfolder, GetType().Name);
        Directory.CreateDirectory(Location);
    }

    protected FacilityBase(string facilityName)
    {
        Location = Path.Combine(AppContext.BaseDirectory, Subfolder, facilityName);
        Directory.CreateDirectory(Location);
    }



    #region Download
    public virtual async Task Download()
    {
        var links = GetDownloadLinks();
        using var downloadManager = new DownloadManager();
        await downloadManager.Download(links, Location);
    }
    public virtual async Task Download(TimeSpan delay)
    {
        var links = GetDownloadLinks();
        using var downloadManager = new DownloadManager();
        await downloadManager.Download(links, Location, delay);
    }
    public virtual List<string> GetDownloadLinks() => GetDownloadLinks(Years.Min, Years.Max, Magnitude.Min, Magnitude.Max);
    public virtual List<string> GetDownloadLinks(int yearMin, int yearMax, int magnitudeMin, int magnitudeMax) => new(0);
    #endregion



    #region Create
    public virtual void MergeData()
    {
        var mergedDataFile = MergedFilePath;
        File.Delete(mergedDataFile);

        var files = Directory.GetFiles(Location)
            .Except(GetWorkFilePaths())
            .ToList();

        if (!files.Any()) return;

        var csvHeader = File.ReadLines(files[0]).First();
        using (var streamWriter = File.AppendText(mergedDataFile))
        {
            streamWriter.WriteLine(csvHeader);
        }

        foreach (var lines in files.Select(file => File.ReadAllLines(file).Skip(1)))
        {
            File.AppendAllLines(mergedDataFile, lines);
        }
    }
    public void ParseAndSave()
    {
        var data = GetRaw()
            .Distinct()
            .ToList();

        var result = ConvertToObj<TC>(data);

        File.Delete(ParsedFilePath);
        Save(result, ParsedFilePath);
    }
    public virtual void FilterAndSave() { }

    public void CountParsedAndSave()
    {
        var parsedData = GetParsed();
        var count = CountEarthQuakesByYears(parsedData);
        SaveParsed(count);
    }
    public void CountFilteredAndSave()
    {
        var data = GetResult();
        var count = CountEarthQuakesByYears(data);
        SaveFiltered(count);
    }

    protected void SaveParsed(List<EqByYearsCount> data) => Save(data, CountParsedFilePath);
    protected void SaveFiltered(List<EqByYearsCount> data) => Save(data, CountResultFilePath);

    protected void Save(List<EqByYearsCount> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Delimiter });
        csv.WriteRecords(data);
    }
    protected void Save(List<EarthquakeInfo> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Delimiter });
        csv.WriteRecords(data);
    }
    #endregion



    #region Read
    public virtual List<EarthquakeRawInfo> GetRaw() => Get<T, EarthquakeRawInfo>(MergedFilePath);
    public virtual List<EarthquakeInfo> GetParsed() => Get<EarthquakeInfo>(ParsedFilePath);
    public virtual List<EarthquakeInfo> GetResult() => Get<EarthquakeInfo>(FilteredFilePath);

    public List<EqByYearsCount> GetCountParsed() => Get<EarthQuakesCountMap, EqByYearsCount>(CountParsedFilePath);
    public List<EqByYearsCount> GetCountResult() => Get<EarthQuakesCountMap, EqByYearsCount>(CountResultFilePath);
    protected List<EqByYearsCount> CountEarthQuakesByYears(List<EarthquakeInfo> data)
    {
        return data
            .GroupBy(info => info.Date.Year)
            .Select(i => new EqByYearsCount(i.Key, i.Count()))
            .OrderBy(i => i.Year)
            .ToList();
    }

    protected virtual List<string> GetWorkFilePaths()
    {
        return new List<string> { MergedFilePath, ParsedFilePath, FilteredFilePath, CountParsedFilePath, CountResultFilePath};
    }

    protected List<TR> Get<TM, TR>(string filePath) where TM : ClassMap where TR : class
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) 
        {
            Delimiter = Delimiter,
            MissingFieldFound = null,
            BadDataFound = null
        });

        csv.Context.RegisterClassMap<TM>();
        return csv.GetRecords<TR>().ToList();
    }
    protected List<TR> Get<TR>(string filePath) where TR : class
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = Delimiter,
            MissingFieldFound = null,
            BadDataFound = null
        });

        return csv.GetRecords<TR>().ToList();
    }
    #endregion



    #region Update
    protected List<EarthquakeInfo> ConvertToObj<TR>(List<EarthquakeRawInfo> data) where TR : IEarthquakeInfoObjConverter, new() => new TR().ConvertToObj(data);
    #endregion



    #region Delete
    public void ClearWorkFolder()
    {
        var di = new DirectoryInfo(Location);

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
    public void DeleteMerge() => File.Delete(MergedFilePath);
    public void DeleteParsed() => File.Delete(ParsedFilePath);
    public void DeleteResult() => File.Delete(FilteredFilePath);
    public void DeleteCountResult() => File.Delete(CountResultFilePath);
    public void DeleteCountMerge() => File.Delete(CountParsedFilePath);
    public virtual void DeleteWorkFiles()
    {
        DeleteMerge();
        DeleteParsed();
        DeleteResult();
        DeleteCountResult();
        DeleteCountMerge();
    }
    #endregion
}
