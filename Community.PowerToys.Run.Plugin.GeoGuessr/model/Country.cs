namespace PowerToys_Run_GeoGuessr;

public class Country
{
    public string domain { get; set; }
    public Flag flag { get; set; }
    public string icon { get; set; }
    public string name { get; set; }
    public string region { get; set; }
    public Road road { get; set; }

    public string Describe()
    {
        return $"Region: {region}\nRoad side: {road.side}\nDomain: {domain}";
    }
}