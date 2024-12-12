using System.Linq;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class TldSingleExtractor : RestCountriesExtractor, ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Tlds"];
    }

    public object? Extract(Country country)
    {
        return country.Cca2 == null ? null : GetCountryInfoByCca2(country.Cca2)?.Tld?.Select(tld => tld[1..]).ToList();
    }
}