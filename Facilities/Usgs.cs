using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;
using EarthquakeMilkShake.Utils;
using System.Diagnostics;

namespace EarthquakeMilkShake.Facilities;

public class Usgs : FacilityBase<UsgsObjMap, UsgsConverter>
{
    public Usgs()
    {
    }

    public Usgs(FacilitySettings settings) : base(settings)
    {
    }

    public override async Task Download()
    {
        using var downloadManager = new DownloadManager();

        for (var i = Settings.Magnitude.Min; i < Settings.Magnitude.Max; i = Math.Round(i + 0.2, 1))
        {
            var (tMgMin, tMgMax) = (i, Math.Round(i + 0.1, 1));
            Debug.WriteLine($"Current mg: {i.ToString(Settings.NumberFormatInfo)}");

            await downloadManager.Download(new DownloadSettings
            {
                UrlList = GetDownloadLinks(Settings.Years.Min, Settings.Years.Max, tMgMin, tMgMax),
                BaseDirectory = Settings.Location,
                FileEndingName = $"mg{tMgMin.ToString(Settings.NumberFormatInfo)}_{tMgMax.ToString(Settings.NumberFormatInfo)}"
            });
        }
    }

    #region Download
    public override List<string> GetDownloadLinks(int yearMin, int yearMax, double magnitudeMin, double magnitudeMax)
    {
        var result = new List<string>
        {
            $"https://earthquake.usgs.gov/fdsnws/event/1/query.csv?starttime={yearMin}-01-01%2000:00:00&endtime={yearMin}-01-01%2000:00:00&minmagnitude={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmagnitude={magnitudeMax.ToString(Settings.NumberFormatInfo)}&eventtype=earthquake&contributor=us&orderby=time-asc"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add($"https://earthquake.usgs.gov/fdsnws/event/1/query.csv?starttime={i}-01-01%2000:00:01&endtime={i + 1}-01-01%2000:00:00&minmagnitude={magnitudeMin.ToString(Settings.NumberFormatInfo)}&maxmagnitude={magnitudeMax.ToString(Settings.NumberFormatInfo)}&eventtype=earthquake&contributor=us&orderby=time-asc");
        }

        return result;
    }
    #endregion



    #region Create
    public override void FilterAndSave()
    {
        var data = GetParsed()
            .Where(i => i.Contributor.Equals("us"))
            .ToList();

        DeleteResult();
        Save(data, Settings.FilteredFilePath);
    }
    #endregion



    #region Update
    protected override string AdjustLocationName(string location)
    {
        location = location.ToLower();

        var countryIndex = location.LastIndexOf(", ", StringComparison.Ordinal);
        if (countryIndex != -1)
        {
            location = location.Substring(countryIndex + 2);
        }

        location = location.Replace(" region", "")
            .Replace("north of the ", "")
            .Replace("south of the ", "")
            .Replace("east of the ", "")
            .Replace("west of the ", "")
            .Replace("northern ", "")
            .Replace("southern ", "")
            .Replace("western ", "")
            .Replace("eastern ", "")
            .Replace("central ", "")
            .Replace("north of ", "")
            .Replace("south of ", "")
            .Replace("east of ", "")
            .Replace("west of ", "")
            .Replace("off the coast of ", "")
            .Replace("off the north coast of ", "")
            .Replace("off the south coast of ", "")
            .Replace("off the east coast of ", "")
            .Replace("off the west coast of ", "")
            .Replace("north island of ", "")
            .Replace("south island of ", "")
            .Replace("east island of ", "")
            .Replace("west island of ", "")
            .Replace("northeast ", "")
            .Replace("northwest ", "")
            .Replace("southeast ", "")
            .Replace("southwest ", "")
            .Replace("near the coast of ", "")

            .Replace("fiji islands", "fiji")

            .Replace("alabama", "usa")
            .Replace("arizona", "usa")
            .Replace("arkansas", "usa")
            .Replace("california", "usa")
            .Replace("colorado", "usa")
            .Replace("connecticut", "usa")
            .Replace("delaware", "usa")
            .Replace("florida", "usa")
            .Replace("georgia", "usa")
            .Replace("hawaii", "usa")
            .Replace("idaho", "usa")
            .Replace("illinois", "usa")
            .Replace("indiana", "usa")
            .Replace("iowa", "usa")
            .Replace("kansas", "usa")
            .Replace("kentucky", "usa")
            .Replace("louisiana", "usa")
            .Replace("maine", "usa")
            .Replace("maryland", "usa")
            .Replace("massachusetts", "usa")
            .Replace("michigan", "usa")
            .Replace("minnesota", "usa")
            .Replace("mississippi", "usa")
            .Replace("missouri", "usa")
            .Replace("montana", "usa")
            .Replace("nebraska", "usa")
            .Replace("nevada", "usa")
            .Replace("new hampshire", "usa")
            .Replace("new jersey", "usa")
            .Replace("new mexico", "usa")
            .Replace("new york", "usa")
            .Replace("north carolina", "usa")
            .Replace("north dakota", "usa")
            .Replace("ohio", "usa")
            .Replace("oklahoma", "usa")
            .Replace("oregon", "usa")
            .Replace("pennsylvania", "usa")
            .Replace("rhode Island", "usa")
            .Replace("south Carolina", "usa")
            .Replace("south Dakota", "usa")
            .Replace("tennessee", "usa")
            .Replace("texas", "usa")
            .Replace("utah", "usa")
            .Replace("vermont", "usa")
            .Replace("virginia", "usa")
            .Replace("washington", "usa")
            .Replace("west Virginia", "usa")
            .Replace("wisconsin", "usa")
            .Replace("wyoming", "usa");

        return location;
    }
    #endregion
}
