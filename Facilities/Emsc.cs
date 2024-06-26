﻿using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.Utils;
using System.Diagnostics;

namespace EarthquakeMilkShake.Facilities;

// https://www.seismicportal.eu/fdsn-wsevent.html
public class Emsc : FacilityBase<EmscObjMap, EmscConverter>
{
    public Emsc()
    {
    }

    public Emsc(FacilitySettings settings) : base(settings)
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
            $"https://www.seismicportal.eu/fdsnws/event/1/query?limit=20000&start={yearMin}-01-01&end={yearMin}-01-01&format=text&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add($"https://www.seismicportal.eu/fdsnws/event/1/query?limit=20000&start={i}-01-02&end={i+1}-01-01&format=text&minmag={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmag={magnitudeMax.ToString(Settings.NumberFormatInfo)}");
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
            csvHeader = csvHeader.Replace("|", ",");
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
        var data = GetParsed()
            .Where(i => i.Contributor.Equals("EMSC"))
            .ToList();

        DeleteResult();
        Save(data, Settings.FilteredFilePath);
    }
}
