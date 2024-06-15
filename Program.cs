using EarthquakeMilkShake.Facilities;

Emsc             emsc             = new Emsc();
IrisIeb          irisIeb          = new IrisIeb();
IrisWilber       irisWilber       = new IrisWilber();
Isc              isc              = new Isc();
Usgs             usgs             = new Usgs();
IrisIebIndonesia irisIebIndonesia = new IrisIebIndonesia();
List<IFacility>  facilities       = new List<IFacility> { emsc, irisIeb, irisWilber, isc, usgs };

void ClearWorkFolder()                 => facilities.ForEach(f => f.ClearWorkFolder());
async Task DownloadAllData()           => await Task.WhenAll(emsc.Download(), irisIeb.Download(), irisWilber.Download(), isc.Download(), usgs.Download());
void MergeData()                       => facilities.ForEach(f => f.MergeData());
void ParseAndSave()                    => facilities.ForEach(f => f.ParseAndSave());
void FilterAndSave()                   => facilities.ForEach(f => f.FilterAndSave());
void CountParsedAndSave()              => facilities.ForEach(f => f.CountParsedAndSave());
void CountFilteredAndSave()            => facilities.ForEach(f => f.CountFilteredAndSave());
void CountByMagnitudeFilteredAndSave() => facilities.ForEach(f => f.CountByMagnitudeFilteredAndSave());
void CountByMagnitudeParsedAndSave()   => facilities.ForEach(f => f.CountByMagnitudeParsedAndSave());
void CountByLocationFilteredAndSave() => facilities.ForEach(f => f.CountByLocationFilteredAndSave());
void CountByLocationParsedAndSave() => facilities.ForEach(f => f.CountByLocationParsedAndSave());
void CountByDepthFilteredAndSave() => facilities.ForEach(f => f.CountByDepthFilteredAndSave());
void CountByDepthParsedAndSave() => facilities.ForEach(f => f.CountByDepthParsedAndSave());

// Preparation step: optionally customize the magnitude and years parameters in this class:
// FacilitySettings

// Step 1: Delete ALL files
// ClearWorkFolder();

// Step 2: Download earthquakes info
// await DownloadAllData();

// Step 3: Merge and parse all the data.
// MergeData();

// Step 3.1: Here the data is converted into a single format
// ParseAndSave();

// Step 3.2: Here the data is filtered by the exact data providers
// FilterAndSave();

// Step 3.3: Count earthquakes by years
// CountByMagnitudeFilteredAndSave();
// CountByMagnitudeParsedAndSave();

// Step 3.4: Count earthquakes by location
// CountByLocationFilteredAndSave();
// CountByLocationParsedAndSave();

// Step 3.5: Count earthquakes by depth
// CountByDepthFilteredAndSave();
// CountByDepthParsedAndSave();

// Additional step: Calculate Indonesia data
// await irisIndonesia.CalculateIndonesiaData();
// irisIndonesia.CountFilteredAndSave(new DateTime(2015, 06, 1), new DateTime(2016, 06, 30));
// irisIndonesia.CountByMagnitudeFilteredAndSave();
// irisIndonesia.CountByMagnitudeParsedAndSave();

return 0;
