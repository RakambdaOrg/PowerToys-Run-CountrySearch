using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class RegionSingleExtractor : RestCountriesExtractor, ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Region"];
    }

    public object? Extract(Country country)
    {
        return country.Cca2 == null ? null : GetCountryInfoByCca2(country.Cca2)?.Region;
    }
}