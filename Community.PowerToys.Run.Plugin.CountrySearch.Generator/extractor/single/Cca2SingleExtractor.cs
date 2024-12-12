using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class Cca2SingleExtractor : RestCountriesExtractor, ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Cca2"];
    }

    public object? Extract(Country country)
    {
        return country.Name == null ? null : GetCountryInfoByName(country.Name)?.Cca2;
    }
}