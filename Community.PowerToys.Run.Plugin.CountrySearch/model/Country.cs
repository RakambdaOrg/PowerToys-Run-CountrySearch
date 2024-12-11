namespace PowerToys_Run_CountrySearch.model;

public class Country
{
    public string domain { get; set; }
    public Flag flag { get; set; }
    public string icon { get; set; }
    public string name { get; set; }
    public string region { get; set; }
    public Phone phone { get; set; }
    public Road road { get; set; }

    public string Describe()
    {
        return $"""
                Region: {region} | Domain: {domain}
                Road side: {road.side} | Phone code: +{phone.code}
                """;
    }
}