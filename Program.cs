using EarthquakeMilkShake.Facilities;

Emsc emsc = new Emsc();
Iris iris = new Iris();
Isc  isc  = new Isc();
Usgs usgs = new Usgs();
IrisIndonesia irisIndonesia = new IrisIndonesia();

FacilityService facilityService = new FacilityService(emsc, iris, isc, usgs);
List<IFacility> facilities = new List<IFacility> { emsc, iris, isc, usgs };

void DeleteWorkFiles() => facilities.ForEach(f => f.DeleteWorkFiles());
async Task DownloadAllData() => await Task.WhenAll(emsc.Download(), iris.Download(), isc.Download(), usgs.Download());
void MergeData() => facilities.ForEach(f => f.MergeData());
void ParseAndSave() => facilities.ForEach(f => f.ParseAndSave());
void FilterAndSave() => facilities.ForEach(f => f.FilterAndSave());
void CountParsedAndSave() => facilities.ForEach(f => f.CountParsedAndSave());
void CountFilteredAndSave() => facilities.ForEach(f => f.CountFilteredAndSave());

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
// CountParsedAndSave();
// CountFilteredAndSave();

// Step 3.4: Combine counted earthquakes into a single file
// facilityService.DeleteAll();
// facilityService.CountCombinedParsedAndSave();
// facilityService.CountCombinedFilteredAndSave();

// Additional step: Calculate Indonesia data
// await irisIndonesia.CalculateIndonesiaData();

// Additional step: Calculate earthquakes count by location from USGS facility
// usgs.DeleteLocationCount();
// usgs.SaveCountByLocationAll();
// usgs.SaveCountByLocationPartial();
return 0;
