namespace PowerToys_Run_CountrySearch.model;

public class Country
{
    public string? Cca2 { get; set; }
    public List<string>? Tlds { get; set; }
    public List<string>? Continents { get; set; }
    public Flag? Flag { get; set; }
    public string? Name { get; set; }
    public string? Region { get; set; }
    public Phone? Phone { get; set; }
    public Road? Road { get; set; }
    public List<Language>? Languages { get; set; }
}