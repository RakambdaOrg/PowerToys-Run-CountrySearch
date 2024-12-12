using System.Linq;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class LanguageSingleExtractor : RestCountriesExtractor, ISingleExtractor
{
    public string[] GetPropertyPath()
    {
        return ["Languages"];
    }

    public object? Extract(Country country)
    {
        return country.Cca2 == null
            ? null
            : GetCountryInfoByCca2(country.Cca2)?.Languages?.Select((entry, _) => new Language
            {
                Code = entry.Key,
                Name = entry.Value
            }).ToList();
    }
}