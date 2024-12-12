using System.Linq;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class ContinentSingleExtractor : RestCountriesExtractor, ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Continents"];
    }

    public object? Extract(Country country)
    {
        return country.Cca2 == null ? null : GetCountryInfoByCca2(country.Cca2)?.Continents?.ToList();
    }
}