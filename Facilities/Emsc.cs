using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;

namespace EarthquakeMilkShake.Facilities;

// https://www.seismicportal.eu/fdsn-wsevent.html
public class Emsc : FacilityBase<EmscObjMap, EmscConverter>
{
    public override List<string> GetDownloadLinks(int yearMin, int yearMax, int magnitudeMin, int magnitudeMax)
    {
        var result = new List<string>
        {
            $"https://www.seismicportal.eu/fdsnws/event/1/query?limit=20000&start={yearMin}-01-01&end={yearMin}-01-01&format=text&minmag={magnitudeMin}&maxmag={magnitudeMax}"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add($"https://www.seismicportal.eu/fdsnws/event/1/query?limit=20000&start={i}-01-02&end={i+1}-01-01&format=text&minmag={magnitudeMin}&maxmag={magnitudeMax}");
        }

        return result;
    }

    public override void MergeData()
    {
        var mergedDataFile = MergedFilePath;
        File.Delete(mergedDataFile);

        var files = Directory.GetFiles(Location).Except(GetWorkFilePaths()).ToList();

        if (!files.Any()) return;

        var csvHeader = File.ReadLines(files[0]).First().Replace("|", ",");
        using (var streamWriter = File.AppendText(mergedDataFile))
        {
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
        var data = GetParsed()
            .Where(i => i.Contributor.Equals("EMSC"))
            .ToList();

        DeleteResult();
        Save(data, FilteredFilePath);
    }
}
