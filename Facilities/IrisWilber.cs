using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.Utils;
using System.Diagnostics;

namespace EarthquakeMilkShake.Facilities;

// http://ds.iris.edu/ds/nodes/dmc/tools/#data_types=events
public class IrisWilber : FacilityBase<IrisWilberObjMap, IrisWilberConverter>
{
    public IrisWilber()
    {
    }

    public IrisWilber(FacilitySettings settings) : base(settings)
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
            $"http://service.iris.edu/fdsnws/event/1/query?starttime={yearMin}-01-01&endtime={yearMin}-01-01T23%3A59%3A59.999999&minmagnitude={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmagnitude={magnitudeMax.ToString(Settings.NumberFormatInfo)}&mindepth=0&maxdepth=6371&limit=10000&output=text"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.AddRange( new []{
                $"http://service.iris.edu/fdsnws/event/1/query?starttime={i}-01-02&endtime={i}-04-01T23%3A59%3A59.999999&minmagnitude={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmagnitude={magnitudeMax.ToString(Settings.NumberFormatInfo)}&mindepth=0&maxdepth=6371&limit=10000&output=text",
                $"http://service.iris.edu/fdsnws/event/1/query?starttime={i}-04-02&endtime={i}-08-01T23%3A59%3A59.999999&minmagnitude={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmagnitude={magnitudeMax.ToString(Settings.NumberFormatInfo)}&mindepth=0&maxdepth=6371&limit=10000&output=text",
                $"http://service.iris.edu/fdsnws/event/1/query?starttime={i}-08-02&endtime={i+1}-01-01T23%3A59%3A59.999999&minmagnitude={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmagnitude={magnitudeMax.ToString(Settings.NumberFormatInfo)}&mindepth=0&maxdepth=6371&limit=10000&output=text"
            });
        }

        return result;
    }

    public override void MergeData()
    {
        var mergedDataFile = Settings.MergedFilePath;
        File.Delete(mergedDataFile);

        var files = Directory.GetFiles(Settings.Location).Except(GetWorkFilePaths()).ToList();

        if (!files.Any()) return;

        var notEmptyFile = files.FirstOrDefault(f => new FileInfo(f).Length != 0);
        if (notEmptyFile is null) return;

        var csvHeader = File.ReadLines(notEmptyFile).FirstOrDefault();
        if (csvHeader != null)
        {
            csvHeader = csvHeader.Replace("|", ",").Replace(" ", "");
            using var streamWriter = File.AppendText(mergedDataFile);
            streamWriter.WriteLine(csvHeader);
        }

        foreach (var lines in files.Select(file => File.ReadAllLines(file).Skip(1)))
        {
            var formattedLines = lines.ToList().Select(line => line.Replace("|", ","));
            File.AppendAllLines(mergedDataFile, formattedLines);
        }
    }

    public override void FilterAndSave()
    {
        var data = GetParsed().Distinct().ToList();

        DeleteResult();
        Save(data, Settings.FilteredFilePath);
    }
}
