using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PowerToys_Run_CountrySearch_Generator.extractor;

public partial class RegionExtractor : IExtractor
{
    private static readonly Regex CountryRegex = CountryRegexGenerator();

    [GeneratedRegex("^(?<country>[^:]+)$")]
    private static partial Regex CountryRegexGenerator();

    public string[] GetJsonPath()
    {
        return ["region"];
    }

    public Dictionary<string, string> Extract()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://geohints.com/Country");

        var areaDivs = doc.DocumentNode.SelectNodes("//div[boolean(@style)][child::h3]");
        foreach (var areaDiv in areaDivs)
        {
            var areaName = areaDiv.SelectSingleNode("./h3").InnerText[..^1];

            var content = areaDiv.InnerText;
            var lines = content.Split('\n');

            var areas = lines
                .Select(line => line.Trim())
                .Select(line => CountryRegex.Match(line))
                .Where(match => match.Success)
                .Select(match => (match.Groups["country"].Value, areaName));

            foreach (var (country, area) in areas)
            {
                values.TryAdd(country, area);
            }
        }

        return values;
    }
}