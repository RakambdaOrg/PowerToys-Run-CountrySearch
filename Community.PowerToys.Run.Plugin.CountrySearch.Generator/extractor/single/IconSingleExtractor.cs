using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;
using PowerToys_Run_CountrySearch;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public class IconSingleExtractor : ISingleExtractor
{
    private static readonly Dictionary<string, string> Names = NamesGenerator();

    public string[] GetJsonPath()
    {
        return ["flag", "file"];
    }

    public string? Extract(Country country)
    {
        return CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(c => new RegionInfo(c.Name))
            .FirstOrDefault(r => r.EnglishName == country.name)
            ?.TwoLetterISORegionName.ToLowerInvariant()
            .Transform(c => $"{c}.png");
    }

    private static Dictionary<string, string> NamesGenerator()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes");

        var tableBody = doc.DocumentNode.SelectSingleNode("//table[@id='Table']/tbody");
        var tableRows = tableBody.SelectNodes("./tr");
        var count = 0;
        foreach (var tableRow in tableRows)
        {
            if (count < 2)
            {
                count++;
                continue;
            }

            var languageCode = tableRow.SelectSingleNode("./td[2]").InnerText;
            var language = tableRow.SelectSingleNode("./td[1]").InnerText.Split(",")[0].Cleanup();

            values.TryAdd(languageCode, language);
        }

        return values;
    }
}