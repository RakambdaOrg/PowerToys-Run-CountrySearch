using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class RoadSideSingleExtractor : RestCountriesExtractor, ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Road", "Side"];
    }

    public object? Extract(Country country)
    {
        return country.Cca2 == null ? null : GetCountryInfoByCca2(country.Cca2)?.Car?.Side;
    }
}