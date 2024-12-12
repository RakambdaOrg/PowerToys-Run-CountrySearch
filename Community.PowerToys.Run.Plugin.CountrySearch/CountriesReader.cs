using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch;

public static class CountriesReader
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static CountriesSchema ReadFile(string path)
    {
        var jsonString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<CountriesSchema>(jsonString, JsonSerializerOptions) ?? new CountriesSchema { Countries = [] };
    }
}