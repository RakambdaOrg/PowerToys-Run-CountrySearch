using System.Globalization;
using System.Linq;
using PowerToys_Run_CountrySearch;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class IconSingleExtractor : ISingleExtractor
{
    public string[] GetJsonPath()
    {
        return ["flag", "file"];
    }

    public object? Extract(Country country)
    {
        return CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(c => new RegionInfo(c.Name))
            .FirstOrDefault(r => r.EnglishName == country.name)
            ?.TwoLetterISORegionName.ToLowerInvariant()
            .Transform(c => $"{c}.png");
    }
}