using PowerToys_Run_CountrySearch;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class FlagFileSingleExtractor : ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Flag", "File"];
    }

    public object? Extract(Country country)
    {
        return country.Cca2?.Transform(c => $"{c.ToLowerInvariant()}.png");
    }
}