using System.Dynamic;
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
    public readonly FacilitySettings Settings;

    protected FacilityBase()
    {
        Settings = new FacilitySettings
        {
            Location = Path.Combine(AppContext.BaseDirectory, "Data", GetType().Name)
        };
    }

    protected FacilityBase(FacilitySettings settings)
    {
        Settings = settings;
    }


    #region Download
    public virtual async Task Download()
    {
        var links = GetDownloadLinks();
        using var downloadManager = new DownloadManager();
        await downloadManager.Download(new DownloadSettings
        {
            UrlList = links,
            BaseDirectory = Settings.Location
        });
    }
    public virtual async Task Download(DownloadSettings settings)
    {
        using var downloadManager = new DownloadManager();
        await downloadManager.Download(settings);
    }
    public virtual List<string> GetDownloadLinks() => GetDownloadLinks(Settings.Years.Min, Settings.Years.Max, Settings.Magnitude.Min, Settings.Magnitude.Max);
    public virtual List<string> GetDownloadLinks(int yearMin, int yearMax, double magnitudeMin, double magnitudeMax) => new(0);
    #endregion



    #region Create
    public virtual void MergeData()
    {
        var mergedDataFile = Settings.MergedFilePath;
        File.Delete(mergedDataFile);

        var files = Directory.GetFiles(Settings.Location)
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
            .DistinctBy(i => new {i.Latitude, i.Longitude, i.Date, i.Magnitude, i.Depth})
            .ToList();

        var result = ConvertToObj<TC>(data);

        File.Delete(Settings.ParsedFilePath);
        Save(result, Settings.ParsedFilePath);
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
        var data = GetFiltered();
        var count = CountEarthQuakesByYears(data);
        SaveFiltered(count);
    }
    public void CountByMagnitudeFilteredAndSave()
    {
        var data = GetFiltered();
        var count = CountEqByMagnitude(data);
        Save(count, Settings.CountByMagFilteredFilePath);
    }
    public void CountByMagnitudeParsedAndSave()
    {
        var data = GetParsed();
        var count = CountEqByMagnitude(data);
        Save(count, Settings.CountByMagParsedFilePath);
    }

    public void CountByLocationFilteredAndSave() => CountByLocationAndSave(Settings.CountByLocationFilteredFileName, GetFiltered());
    public void CountByLocationParsedAndSave() => CountByLocationAndSave(Settings.CountByLocationParsedFileName, GetParsed());
    protected void CountByLocationAndSave(string fileName, List<EarthquakeInfo> data)
    {
        var result = GetEqByLocation(data);
        Save(result, Path.Combine(Settings.Location, fileName));
    }

    public void CountByDepthFilteredAndSave(string country = "") => CountByDepthAndSave(Settings.CountByDepthFilteredFileName, GetFiltered(), country);
    public void CountByDepthParsedAndSave(string country = "") => CountByDepthAndSave(Settings.CountByDepthParsedFileName, GetParsed(), country);
    protected void CountByDepthAndSave(string fileName, List<EarthquakeInfo> data, string country = "")
    {
        var result = GetEqByDepth(data, country);
        Save(result, Path.Combine(Settings.Location, fileName));
    }

    protected void SaveParsed(List<EqByYearsCount> data) => Save(data, Settings.CountParsedFilePath);
    protected void SaveFiltered(List<EqByYearsCount> data) => Save(data, Settings.CountResultFilePath);

    protected void Save(List<EqByYearsCount> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Settings.Delimiter });
        csv.WriteRecords(data);
    }
    protected void Save(List<EarthquakeInfo> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Settings.Delimiter });
        csv.WriteRecords(data);
    }
    protected void Save(List<EqMagnitudesCount> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Settings.Delimiter });
        csv.WriteRecords(data);
    }
    protected void Save(List<object> data, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = Settings.Delimiter });
        csv.WriteRecords(data);
    }
    #endregion



    #region Read
    public virtual List<EarthquakeRawInfo> GetRaw() => Get<T, EarthquakeRawInfo>(Settings.MergedFilePath);
    public virtual List<EarthquakeInfo> GetParsed() => Get<EarthquakeInfo>(Settings.ParsedFilePath);
    public virtual List<EarthquakeInfo> GetFiltered() => Get<EarthquakeInfo>(Settings.FilteredFilePath);

    public List<EqByYearsCount> GetCountParsed() => Get<EarthQuakesCountMap, EqByYearsCount>(Settings.CountParsedFilePath);
    public List<EqByYearsCount> GetCountResult() => Get<EarthQuakesCountMap, EqByYearsCount>(Settings.CountResultFilePath);
    protected List<EqByYearsCount> CountEarthQuakesByYears(List<EarthquakeInfo> data)
    {
        return data.GroupBy(i => i.Date.Year)
                   .Select(i => new EqByYearsCount(i.Key, i.Count()))
                   .OrderBy(i => i.Year)
                   .ToList();
    }

    public List<object> GetEqByLocation(List<EarthquakeInfo> data)
    {
        var result = new List<object>();

        var groupedPlaces = data
            .GroupBy(d => AdjustLocationName(d.Place))
            .ToDictionary(d => d.Key, d => d)
            .OrderByDescending(pair => pair.Value.Count());
        
        foreach (var (place, earthQuakeInfo) in groupedPlaces)
        {
            var earthquakesDynamic = new ExpandoObject() as IDictionary<string, object>;
            earthquakesDynamic.Add("Country", place);

            var populatedYears = new Dictionary<int, int>();
            for (var i = Settings.Years.Min; i <= Settings.Years.Max; i++)
            {
                populatedYears.Add(i, 0);
            }

            var countByYear = earthQuakeInfo
                .GroupBy(info => info.Date.Year)
                .ToDictionary(info => info.Key, info => info.ToList().Count);
            
            foreach (var (year, eqCount) in countByYear)
            {
                populatedYears[year] = eqCount;
            }

            foreach (var (year, eqCount) in populatedYears)
            {
                earthquakesDynamic.Add(year.ToString(), eqCount);
            }

            earthquakesDynamic.Add("TotalCount", countByYear.Values.Sum());

            result.Add(earthquakesDynamic);
        }

        return result;
    }

    public List<object> GetEqByDepth(List<EarthquakeInfo> data, string country = "")
    {
        var result = new List<object>();

        var groupedDepths = new Dictionary<double, List<EarthquakeInfo>>();
        for (var i = -50; i < 1000; i+=25)
        {
            groupedDepths.Add(i, new List<EarthquakeInfo>());
        }

        if (!string.IsNullOrEmpty(country))
        {
            data.ForEach(d =>
            {
                AdjustLocationName(d.Place);

                if (d.Place.ToLower().Equals(country.ToLower()))
                {
                    groupedDepths[Math.Floor(d.Depth / 25) * 25].Add(d);
                }
            });
        }
        else
        {
            data.ForEach(d =>
            {
                groupedDepths[Math.Floor(d.Depth / 25) * 25].Add(d);
            });
        }

        foreach (var (depth, earthQuakeInfo) in groupedDepths)
        {
            var earthquakesDynamic = new ExpandoObject() as IDictionary<string, object>;
            earthquakesDynamic.Add("Depth", depth);

            var populatedYears = new Dictionary<int, int>();
            for (var i = Settings.Years.Min; i <= Settings.Years.Max; i++)
            {
                populatedYears.Add(i, 0);
            }

            var countByDepth = earthQuakeInfo
                .GroupBy(info => info.Date.Year)
                .ToDictionary(info => info.Key, info => info.ToList().Count);

            foreach (var (year, eqCount) in countByDepth)
            {
                populatedYears[year] = eqCount;
            }

            foreach (var (year, eqCount) in populatedYears)
            {
                earthquakesDynamic.Add(year.ToString(), eqCount);
            }

            earthquakesDynamic.Add("TotalCount", countByDepth.Values.Sum());

            result.Add(earthquakesDynamic);
        }

        return result;
    }

    public List<EqMagnitudesCount> CountEqByMagnitude(List<EarthquakeInfo> data)
    {
        var resultDict = new Dictionary<int, EqMagnitudesCount>();

        for (var i = Settings.Years.Min; i <= Settings.Years.Max; i++)
        {
            resultDict.Add(i, new EqMagnitudesCount { Year = i });
        }

        data.ForEach(i =>
        {
            if (i.Date.Year < Settings.Years.Min || i.Date.Year > Settings.Years.Max) return;
            
            switch (i.Magnitude)
            {
                case >= 1 and < 2:  resultDict[i.Date.Year].One++;   return;
                case >= 2 and < 3:  resultDict[i.Date.Year].Two++;   return;
                case >= 3 and < 4:  resultDict[i.Date.Year].Three++; return;
                case >= 4 and < 5:  resultDict[i.Date.Year].Four++;  return;
                case >= 5 and < 6:  resultDict[i.Date.Year].Five++;  return;
                case >= 6 and < 7:  resultDict[i.Date.Year].Six++;   return;
                case >= 7 and < 8:  resultDict[i.Date.Year].Seven++; return;
                case >= 8 and < 9:  resultDict[i.Date.Year].Eight++; return;
                case >= 9 and < 10: resultDict[i.Date.Year].Nine++;  return;
                case >= 10:         resultDict[i.Date.Year].Ten++;
                    break;
            }
        });

        return resultDict.Values.ToList();
    }

    protected virtual List<string> GetWorkFilePaths()
    {
        return new List<string>
        {
            Settings.MergedFilePath, 
            Settings.ParsedFilePath, 
            Settings.FilteredFilePath, 
            Settings.CountParsedFilePath, 
            Settings.CountResultFilePath, 
            Settings.CountByMagFilteredFilePath,
            Settings.CountByMagParsedFilePath,
            Settings.CountByLocationFilteredFilePath,
            Settings.CountByLocationParsedFilePath,
            Settings.CountByLocationPartialFilePath,
            Settings.CountByDepthParsedFilePath
        };
    }

    protected List<TR> Get<TM, TR>(string filePath) where TM : ClassMap where TR : class
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) 
        {
            Delimiter = Settings.Delimiter,
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
            Delimiter = Settings.Delimiter,
            MissingFieldFound = null,
            BadDataFound = null
        });

        return csv.GetRecords<TR>().ToList();   
    }
    #endregion



    #region Update
    protected List<EarthquakeInfo> ConvertToObj<TR>(List<EarthquakeRawInfo> data) where TR : IEarthquakeInfoObjConverter, new() => new TR().ConvertToObj(data);

    protected virtual string AdjustLocationName(string location)
    {
        return location;
    }
    #endregion



    #region Delete
    public void ClearWorkFolder()
    {
        var di = new DirectoryInfo(Settings.Location);
        if (!di.Exists)
        {
            Directory.CreateDirectory(Settings.Location);
            return;
        }

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
    public void DeleteMerge() => File.Delete(Settings.MergedFilePath);
    public void DeleteParsed() => File.Delete(Settings.ParsedFilePath);
    public void DeleteResult() => File.Delete(Settings.FilteredFilePath);
    public void DeleteCountResult() => File.Delete(Settings.CountResultFilePath);
    public void DeleteCountMerge() => File.Delete(Settings.CountParsedFilePath);
    public void DeleteCountByMagFiltered() => File.Delete(Settings.CountByMagFilteredFilePath);
    public void DeleteCountByMagParsed() => File.Delete(Settings.CountByMagParsedFilePath);
    public void DeleteCountLocationAll() => File.Delete(Settings.CountByLocationFilteredFilePath);
    public void DeleteCountLocationParsed() => File.Delete(Settings.CountByLocationParsedFilePath);
    public void DeleteCountLocationPartial() => File.Delete(Settings.CountByLocationPartialFilePath);
    public void DeleteCountByDepthFiltered() => File.Delete(Settings.CountByDepthFilteredFilePath);
    public void DeleteCountByDepthParsed() => File.Delete(Settings.CountByDepthParsedFilePath);

    public virtual void DeleteWorkFiles()
    {
        DeleteMerge();
        DeleteParsed();
        DeleteResult();
        DeleteCountResult();
        DeleteCountMerge();
        DeleteCountByMagFiltered();
        DeleteCountByMagParsed();
        DeleteCountLocationAll();
        DeleteCountLocationParsed();
        DeleteCountLocationPartial();
        DeleteCountByDepthFiltered();
        DeleteCountByDepthParsed();
    }
    #endregion
}
