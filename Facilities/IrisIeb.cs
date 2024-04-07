using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.Utils;
using System.Diagnostics;

namespace EarthquakeMilkShake.Facilities;

// http://ds.iris.edu/ds/nodes/dmc/tools/#data_types=events
public class IrisIeb : FacilityBase<IrisIebObjMap, IrisIebConverter>
{
    public IrisIeb()
    {
    }

    public IrisIeb(FacilitySettings settings) : base(settings)
    {
    }

    public override async Task Download()
    {
        using var downloadManager = new DownloadManager();

        for (var i = Settings.Magnitude.Min; i <= Settings.Magnitude.Max; i = Math.Round(i + 0.1, 1))
        {
            Debug.WriteLine($"Current mg: {i.ToString(Settings.NumberFormatInfo)}");

            await downloadManager.Download(new DownloadSettings
            {
                UrlList = GetDownloadLinks(Settings.Years.Min, Settings.Years.Max, i, i),
                BaseDirectory = Settings.Location,
                FileEndingName = $"mg{i.ToString(Settings.NumberFormatInfo)}_{i.ToString(Settings.NumberFormatInfo)}"
            });
        }
    }

    public override List<string> GetDownloadLinks(int yearMin, int yearMax, double magnitudeMin, double magnitudeMax)
    {
        var result = new List<string>
        {
            $"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={yearMin}-01-01&et={yearMin}-01-01&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&orderby=time-desc&src=iris&limit=25000&maxlat=89.10&minlat=-89.14&maxlon=180.00&minlon=-180.00&pbl=1&zm=1&mt=ter&title=IEB%20export%3A%201000%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{yearMin}-01-01%20to%20{yearMin}-01-01%2C%20with%20magnitudes%20from%20{magnitudeMin.ToString(Settings.NumberFormatInfo)}%20to%20{magnitudeMax.ToString(Settings.NumberFormatInfo)}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2025000%2C%20%20showing%20data%20from%20IRIS%2C%20"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.AddRange( new []{
                $"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={i}-01-02&et={i}-06-01&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&orderby=time-desc&src=iris&limit=25000&maxlat=89.10&minlat=-89.14&maxlon=180.00&minlon=-180.00&pbl=1&zm=1&mt=ter&title=IEB%20export%3A%201000%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{i}-01-02%20to%20{i}-06-01%2C%20with%20magnitudes%20from%20{magnitudeMin.ToString(Settings.NumberFormatInfo)}%20to%20{magnitudeMax.ToString(Settings.NumberFormatInfo)}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2025000%2C%20%20showing%20data%20from%20IRIS%2C%20",
                $"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={i}-06-02&et={i+1}-01-01&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&orderby=time-desc&src=iris&limit=25000&maxlat=89.10&minlat=-89.14&maxlon=180.00&minlon=-180.00&pbl=1&zm=1&mt=ter&title=IEB%20export%3A%201000%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{i}-06-02%20to%20{i+1}-01-01%2C%20with%20magnitudes%20from%20{magnitudeMin.ToString(Settings.NumberFormatInfo)}%20to%20{magnitudeMax.ToString(Settings.NumberFormatInfo)}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2025000%2C%20%20showing%20data%20from%20IRIS%2C%20"
            });
        }

        return result;
    }

    public override void MergeData()
    {
        var mergedDataFile = Settings.MergedFilePath;
        File.Delete(mergedDataFile);

        var files = Directory.GetFiles(Settings.Location)
            .Except(GetWorkFilePaths())
            .ToList();

        if (!files.Any()) return;

        var notEmptyFile = files.FirstOrDefault(f =>
        {
            var firstLine = File.ReadLines(f).FirstOrDefault();
            return firstLine != null && !firstLine.StartsWith("<p>Oops: could not");
        });

        if (notEmptyFile is null) return;

        var csvHeader = File.ReadLines(notEmptyFile).First();
        using (var streamWriter = File.AppendText(mergedDataFile))
        {
            streamWriter.WriteLine(csvHeader);
        }

        foreach (var lines in files.Select(file => File.ReadAllLines(file).Skip(1)))
        {
            var content = lines as string[] ?? lines.ToArray();
            if (content.Length == 0 || content.First().StartsWith("<p>Oops: could not")) continue;
         
            File.AppendAllLines(mergedDataFile, content);
        }
    }

    public override void FilterAndSave()
    {
        var data = GetParsed().ToList().DistinctBy(i => new { i.Date, i.Latitude, i.Longitude, i.Depth, i.Magnitude }).ToList();
        DeleteResult();
        Save(data, Settings.FilteredFilePath);
    }
}
