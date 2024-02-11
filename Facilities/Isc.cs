using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;

namespace EarthquakeMilkShake.Facilities;

// http://www.isc.ac.uk/iscbulletin/search/catalogue/
public class Isc : FacilityBase<IscObjMap, IscConverter>
{
    public override List<string> GetDownloadLinks(int yearMin, int yearMax, int magnitudeMin, int magnitudeMax)
    {
        var result = new List<string>
        {
            $"http://www.isc.ac.uk/cgi-bin/web-db-run?out_format=CATCSV&request=COMPREHENSIVE&searchshape=GLOBAL&start_year={yearMin}&start_month=01&start_day=01&start_time=00:00:00&end_year={yearMin}&end_month=01&end_day=01&end_time=00:00:00&min_mag={magnitudeMin}&max_mag={magnitudeMax}&req_mag_agcy=ISC&req_mag_type=Any"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add($"http://www.isc.ac.uk/cgi-bin/web-db-run?out_format=CATCSV&request=COMPREHENSIVE&searchshape=GLOBAL&start_year={i}&start_month=01&start_day=01&start_time=00:00:01&end_year={i+1}&end_month=01&end_day=01&end_time=00:00:00&min_mag={magnitudeMin}&max_mag={magnitudeMax}&req_mag_agcy=ISC&req_mag_type=Any");
        }

        return result;
    }

    public override void MergeData()
    {
        var mergedDataFile = MergedFilePath;
        File.Delete(mergedDataFile);

        var files = Directory.GetFiles(Location).Except(new[] { MergedFilePath, ParsedFilePath, FilteredFilePath, CountParsedFilePath, CountResultFilePath }).ToList();

        if (!files.Any()) return;

        if (!WriteCsvHeader(files, mergedDataFile)) return;

        foreach (var f in files)
        {
            var lines = File.ReadAllLines(f).ToList();
            var headerIndex = lines.FindIndex(l => l.StartsWith("----EVENT-----"));
            if (headerIndex == -1) return;
            var endIndex = lines.FindIndex(l => l.StartsWith("STOP"));
            if (endIndex == -1) return;

            var separatedLines = lines.GetRange((headerIndex + 2), endIndex - 3 - headerIndex + 1);
            separatedLines = separatedLines.Select(l => l.Replace(" ", "")).ToList();

            File.AppendAllLines(mergedDataFile, separatedLines);
        }
    }

    private static bool WriteCsvHeader(IReadOnlyList<string> files, string mergedDataFile)
    {
        var lines = File.ReadLines(files[0]).ToList();
        var headerIndex = lines.FindIndex(l => l.StartsWith("----EVENT-----"));
        if (headerIndex == -1) return false;
        var formattedHeader = lines[headerIndex + 1].Replace(" ", "");
        using var streamWriter = File.AppendText(mergedDataFile);
        streamWriter.WriteLine(formattedHeader);
        return true;
    }

    public override void FilterAndSave()
    {
        var data = GetParsed()
            .Where(i => i.Contributor.Equals("ISC"))
            .ToList();

        DeleteResult();
        Save(data, FilteredFilePath);
    }
}
