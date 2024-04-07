using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EarthquakeMilkShake.DataRaw;
using EarthquakeMilkShake.EarthquakeCount;

namespace EarthquakeMilkShake.Facilities;

public class IrisIebIndonesia : IrisIeb
{
    protected string DateDetailedFileName { get; set; } = "DateDetailed.csv";
    protected string DateDetailedFilePath => Path.Combine(Settings.Location, DateDetailedFileName);

    protected string MagnitudeDetailedFileName { get; set; } = "MagnitudeDetailed.csv";
    protected string MagnitudeDetailedFilePath => Path.Combine(Settings.Location, MagnitudeDetailedFileName);



    #region Download
    public override List<string> GetDownloadLinks(int yearMin, int yearMax, double magnitudeMin, double magnitudeMax)
    {
        var result = new List<string>()
        {
            $"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={yearMin}-01-01&et={yearMin}-01-01&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&orderby=time-desc&src=iris&limit=25000&maxlat=5.485&minlat=-11.222&maxlon=140.978&minlon=93.341&sbl=1&zm=4&mt=ter&title=IEB%20export%3A%201463%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{Settings.Years.Min}-01-01%20to%20{Settings.Years.Min}-01-01%2C%20with%20magnitudes%20from%20{magnitudeMin.ToString(Settings.NumberFormatInfo)}%20to%20{magnitudeMax.ToString(Settings.NumberFormatInfo)}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2025000%2C%20%20showing%20data%20from%20IRIS%2C%20"
        };


        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add($"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={i}-01-02&et={i + 1}-01-01&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&orderby=time-desc&src=iris&limit=25000&maxlat=5.485&minlat=-11.222&maxlon=140.978&minlon=93.341&sbl=1&zm=4&mt=ter&title=IEB%20export%3A%201463%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{i}-01-02%20to%20{i + 1}-01-01%2C%20with%20magnitudes%20from%20{magnitudeMin.ToString(Settings.NumberFormatInfo)}%20to%20{magnitudeMax.ToString(Settings.NumberFormatInfo)}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2025000%2C%20%20showing%20data%20from%20IRIS%2C%20");
        }

        return result;
    }

    public override List<string> GetDownloadLinks() => GetDownloadLinks(Settings.Years.Min, Settings.Years.Max, 1, 10);
    #endregion



    #region Create
    public async Task CalculateIndonesiaData()
    {
        // Step 1: Delete ALL files
        ClearWorkFolder();

        // Step 2: Download earthquakes info
        await Download();

        // Step 3: Merge and parse all the data.
        // Note: Some files may be empty after downloading. You will have to delete them manually
        MergeData();

        // Step 3.1: Here the data is converted into a single format
        ParseAndSave();

        // Step 3.2: Here the data is filtered by the exact data providers
        FilterAndSave();

        // Step 3.3: Count earthquakes by years
        CountParsedAndSave();
        CountFilteredAndSave();

        // Step 3.4: Get detailed info of 2015-2016
        CountFilteredAndSave(new DateTime(2015, 06, 1), new DateTime(2016, 06, 30));
        CountByMagnitudeFilteredAndSave();
        CountByMagnitudeParsedAndSave();
    }

    public override void FilterAndSave()
    {
        var data = GetParsed().ToList();

        DeleteResult();
        Save(data, Settings.FilteredFilePath);
    }

    public void CountFilteredAndSave(DateTime minDate, DateTime maxDate)
    {
        var data = GetFiltered();
        var count = CountEqByYears(data, minDate, maxDate);
        Save(count, DateDetailedFilePath);
    }

    protected void Save(List<EqDateDetailedCount> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Settings.Delimiter });
        csv.WriteRecords(data);
    }
    #endregion



    #region Read
    protected override List<string> GetWorkFilePaths()
    {
        var baseFiles = base.GetWorkFilePaths();
        baseFiles.Add(DateDetailedFilePath);
        baseFiles.Add(MagnitudeDetailedFilePath);
        return baseFiles;
    }

    protected List<EqDateDetailedCount> CountEqByYears(List<EarthquakeInfo> data, DateTime minDate, DateTime maxDate)
    {
        var resultDict = new Dictionary<DateTime, EqDateDetailedCount>();

        var currentTicks = minDate.Ticks;
        while (true)
        {
            if (currentTicks > maxDate.Ticks) break;
            var processingDay = new DateTime(currentTicks);
            resultDict.Add(processingDay.Date, new EqDateDetailedCount(processingDay.Date, processingDay.Year, processingDay.Month, processingDay.Day, 0));
            currentTicks = processingDay.AddDays(1).Ticks;
        }

        data
            .Where(i => i.Date.Date >= minDate.Date && i.Date.Date <= maxDate.Date)
            .GroupBy(i => i.Date.Date)
            .ToDictionary(i => i.Key, i => i.ToList())
            .Select(i => new EqDateDetailedCount(i.Key.Date, i.Key.Year, i.Key.Month, i.Key.Day, i.Value.Count))
            .ToList()
            .ForEach(o => resultDict[o.Date].Count = o.Count);

        return resultDict.Values.ToList();
    }
    #endregion



    #region Delete
    public void DeleteDateDetailed() => File.Delete(DateDetailedFilePath);
    public void DeleteMagnitudeDetailed() => File.Delete(MagnitudeDetailedFilePath);
    public override void DeleteWorkFiles()
    {
        DeleteDateDetailed();
        DeleteMagnitudeDetailed();
        base.DeleteWorkFiles();
    }
    #endregion
}
