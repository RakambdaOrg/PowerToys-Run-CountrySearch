using System.Globalization;
using System.Linq;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class LanguageSingleExtractor : ISingleExtractor
{
    public string[] GetJsonPath()
    {
        return ["language", "main"];
    }

    public string? Extract(Country country)
    {
        var culture = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .FirstOrDefault(c => new RegionInfo(c.Name).EnglishName == country.name);

        if (culture == null)
        {
            return null;
        }

        return culture.IsNeutralCulture ? culture.EnglishName : culture.Parent.EnglishName;
    }
}