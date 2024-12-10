using System.IO;
using System.Text.Json;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch;

public class CountriesReader
{
    public static Countries ReadFile(string path)
    {
        var jsonString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Countries>(jsonString) ?? new Countries { countries = [] };
    }
}