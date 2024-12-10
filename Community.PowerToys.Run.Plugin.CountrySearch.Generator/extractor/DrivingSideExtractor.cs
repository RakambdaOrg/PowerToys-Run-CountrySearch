using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PowerToys_Run_CountrySearch_Generator.extractor;

public partial class DrivingSideExtractor : IExtractor
{
    private static readonly Regex SideRegex = SideRegexGenerator();
    
    [GeneratedRegex("^(?<country>(.+?)): (?<side>(?:Right|Left))")]
    private static partial Regex SideRegexGenerator();

    public string[] GetJsonPath()
    {
        return ["road", "side"];
    }

    public Dictionary<string, string> Extract()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://geohints.com/Driving");

        var content = doc.DocumentNode.InnerText;
        var lines = content.Split('\n');

        var sides = lines
            .Select(line => line.Trim())
            .Select(line => SideRegex.Match(line))
            .Where(match => match.Success)
            .Select(match => (match.Groups["country"].Value, match.Groups["side"].Value));

        foreach (var (country, side) in sides)
        {
            values.TryAdd(country, side);
        }

        return values;
    }
}