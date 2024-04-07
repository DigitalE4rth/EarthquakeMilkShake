using System.Diagnostics;
using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.Utils;

namespace EarthquakeMilkShake.Facilities;

// http://www.isc.ac.uk/iscbulletin/search/catalogue/
public class Isc : FacilityBase<IscObjMap, IscConverter>
{
    public Isc()
    {
    }

    public Isc(FacilitySettings settings) : base(settings)
    {
    }

    public override async Task Download()
    {
        using var downloadManager = new DownloadManager();

        for (var i = Settings.Magnitude.Min; i < Settings.Magnitude.Max; i = Math.Round(i+0.2, 1))
        {
            var (tMgMin, tMgMax) = (i, Math.Round(i + 0.1, 1));
            Debug.WriteLine($"Current mg: {i.ToString(Settings.NumberFormatInfo)}");

            await downloadManager.Download(new DownloadSettings
            {
                UrlList = GetDownloadLinks(Settings.Years.Min, Settings.Years.Max, tMgMin, tMgMax),
                BaseDirectory = Settings.Location,
                Delay = TimeSpan.FromSeconds(65),
                FileEndingName = $"mg{tMgMin.ToString(Settings.NumberFormatInfo)}_{tMgMax.ToString(Settings.NumberFormatInfo)}"
            });
        }
    }

    public override List<string> GetDownloadLinks(int yearMin, int yearMax, double magnitudeMin, double magnitudeMax)
    {
        var result = new List<string>
        {
            $"http://www.isc.ac.uk/cgi-bin/web-db-run?request=COMPREHENSIVE&out_format=CATCSV&bot_lat=&top_lat=&left_lon=&right_lon=&ctr_lat=&ctr_lon=&radius=&max_dist_units=deg&searchshape=GLOBAL&srn=&grn=&start_year={yearMin}&start_month=1&start_day=01&start_time=00%3A00%3A00&end_year={yearMin}&end_month=1&end_day=01&end_time=00%3A00%3A00&min_dep=&max_dep=&min_mag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&max_mag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&req_mag_type=Any&req_mag_agcy=Any"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.AddRange(new []
            {
                $"http://www.isc.ac.uk/cgi-bin/web-db-run?request=COMPREHENSIVE&out_format=CATCSV&bot_lat=&top_lat=&left_lon=&right_lon=&ctr_lat=&ctr_lon=&radius=&max_dist_units=deg&searchshape=GLOBAL&srn=&grn=&start_year={i}&start_month=1&start_day=01&start_time=00%3A00%3A01&end_year={i}&end_month=6&end_day=01&end_time=00%3A00%3A00&min_dep=&max_dep=&min_mag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&max_mag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&req_mag_type=Any&req_mag_agcy=Any",
                $"http://www.isc.ac.uk/cgi-bin/web-db-run?request=COMPREHENSIVE&out_format=CATCSV&bot_lat=&top_lat=&left_lon=&right_lon=&ctr_lat=&ctr_lon=&radius=&max_dist_units=deg&searchshape=GLOBAL&srn=&grn=&start_year={i}&start_month=6&start_day=01&start_time=00%3A00%3A01&end_year={i+1}&end_month=1&end_day=01&end_time=00%3A00%3A00&min_dep=&max_dep=&min_mag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&max_mag={magnitudeMax.ToString(Settings.NumberFormatInfo)}&req_mag_type=Any&req_mag_agcy=Any"
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

        if (!WriteCsvHeader(files, mergedDataFile)) return;

        foreach (var f in files)
        {
            var lines = File.ReadAllLines(f).ToList();
            var headerIndex = lines.FindIndex(l => l.StartsWith("----EVENT-----"));
            if (headerIndex == -1) continue;
            var endIndex = lines.FindIndex(l => l.StartsWith("STOP"));
            if (endIndex == -1) continue;

            var separatedLines = lines.GetRange((headerIndex + 2), endIndex - 3 - headerIndex + 1);
            separatedLines = separatedLines.Select(l => l.Replace(" ", "")).ToList();

            File.AppendAllLines(mergedDataFile, separatedLines);
        }
    }

    private static bool WriteCsvHeader(IReadOnlyList<string> files, string mergedDataFile)
    {
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file).ToList();
            var headerIndex = lines.FindIndex(l => l.StartsWith("----EVENT-----"));
            if (headerIndex == -1) continue;
            var formattedHeader = lines[headerIndex + 1].Replace(" ", "");
            using var streamWriter = File.AppendText(mergedDataFile);
            streamWriter.WriteLine(formattedHeader);
            return true;
        }

        return false;
    }

    public override void FilterAndSave()
    {
        var data = GetParsed()
            .Where(i => i.Contributor.Equals("ISC"))
            .ToList();

        DeleteResult();
        Save(data, Settings.FilteredFilePath);
    }
}
