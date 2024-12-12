using System.Linq;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class MainLanguageSingleExtractor : LanguageSingleExtractor, ISingleExtractor
{
    public string[] GetJsonPath()
    {
        return ["language", "main"];
    }

    public object? Extract(Country country)
    {
        return country.domain == null ? null : GetCountryInfo(country.domain)?.languages.Values.First();
    }
}