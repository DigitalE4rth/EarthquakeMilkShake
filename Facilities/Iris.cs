using EarthquakeMilkShake.Converters;
using EarthquakeMilkShake.CsvToObjMappers;

namespace EarthquakeMilkShake.Facilities;

public class Iris : FacilityBase<IrisObjMap, IrisConverter>
{
    public override List<string> GetDownloadLinks(int yearMin, int yearMax, int magnitudeMin, int magnitudeMax)
    {
        var result = new List<string>
        {
            $"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={yearMin}-01-01&et={yearMin}-01-01&minmag={magnitudeMin}&maxmag={magnitudeMax}&orderby=time-desc&src=iris&limit=20000&maxlat=89.10&minlat=-89.14&maxlon=180.00&minlon=-180.00&pbl=1&zm=1&mt=ter&title=IEB%20export%3A%201000%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{yearMin}-01-01%20to%20{yearMin}-01-01%2C%20with%20magnitudes%20from%20{magnitudeMin}%20to%20{magnitudeMax}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2020000%2C%20%20showing%20data%20from%20IRIS%2C%20"
        };

        for (var i = yearMin; i <= yearMax; i++)
        {
            result.Add(
                $"http://ds.iris.edu/ieb/events2csv.phtml?caller=IEB&&st={i}-01-02&et={i+1}-01-01&minmag={magnitudeMin}&maxmag={magnitudeMax}&orderby=time-desc&src=iris&limit=20000&maxlat=89.10&minlat=-89.14&maxlon=180.00&minlon=-180.00&pbl=1&zm=1&mt=ter&title=IEB%20export%3A%201000%20earthquakes%20as%20a%20sortable%20table.&stitle=from%20{i}-01-02%20to%20{i+1}-01-01%2C%20with%20magnitudes%20from%20{magnitudeMin}%20to%20{magnitudeMax}%2C%20all%20depths%2C%20with%20priority%20for%20most%20recent%2C%20limited%20to%2020000%2C%20%20showing%20data%20from%20IRIS%2C%20");
        }

        return result;
    }

    public override void FilterAndSave()
    {
        var data = GetParsed().ToList();

        DeleteResult();
        Save(data, FilteredFilePath);
    }
}
