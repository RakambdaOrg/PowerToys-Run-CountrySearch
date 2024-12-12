using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PowerToys_Run_CountrySearch;

namespace PowerToys_Run_CountrySearch_Generator.extractor.mass;

public partial class CountryMassExtractor : IMassExtractor
{
    private static readonly Regex CountryRegex = CountryRegexGenerator();

    [GeneratedRegex("^(?<country>[^:]+)$")]
    private static partial Regex CountryRegexGenerator();

    public string[] GetPropertyPath()
    {
        return [];
    }

    public Dictionary<string, string> Extract()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://geohints.com/Country");

        var areaDivs = doc.DocumentNode.SelectNodes("//div[boolean(@style)][child::h3]");
        foreach (var areaDiv in areaDivs)
        {
            var content = areaDiv.InnerText;
            var lines = content.Split('\n');

            var countries = lines
                .Select(line => line.Trim())
                .Select(line => CountryRegex.Match(line))
                .Where(match => match.Success)
                .Select(match => match.Groups["country"].Value.Cleanup());

            foreach (var country in countries)
            {
                values.TryAdd(country, "fake");
            }
        }

        return values;
    }
}