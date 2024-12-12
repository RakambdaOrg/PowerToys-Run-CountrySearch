using System.Linq;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class OtherLanguageSingleExtractor : LanguageSingleExtractor, ISingleExtractor
{
    public string[] GetJsonPath()
    {
        return ["language", "other"];
    }

    public object? Extract(Country country)
    {
        return country.domain == null ? null : GetCountryInfo(country.domain)?.languages.Values.Skip(1).ToArray();
    }
}