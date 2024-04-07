namespace EarthquakeMilkShake.Facilities;

public interface IFacility
{
    public void ClearWorkFolder();
    public void DeleteWorkFiles();
    public Task Download();
    public void MergeData();
    public void ParseAndSave();
    public void FilterAndSave();
    public void CountParsedAndSave();
    public void CountFilteredAndSave();
    public void CountByMagnitudeFilteredAndSave();
    public void CountByMagnitudeParsedAndSave();
}
