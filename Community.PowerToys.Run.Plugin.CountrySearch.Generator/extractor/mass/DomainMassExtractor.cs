﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PowerToys_Run_CountrySearch;

namespace PowerToys_Run_CountrySearch_Generator.extractor.mass;

public partial class DomainMassExtractor : IMassExtractor
{
    private static readonly Regex DomainRegex = DomainRegexGenerator();

    [GeneratedRegex(@"^(?<country>(.+?)) - (?:\.(?<domain>\w+))")]
    private static partial Regex DomainRegexGenerator();

    public string[] GetJsonPath()
    {
        return ["domain"];
    }

    public Dictionary<string, string> Extract()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://geohints.com/Domains");

        var content = doc.DocumentNode.InnerText;
        var lines = content.Split('\n');

        var domains = lines
            .Select(line => line.Trim())
            .Select(line => DomainRegex.Match(line))
            .Where(match => match.Success)
            .Select(match => (match.Groups["country"].Value.Cleanup(), Domain: match.Groups["domain"].Value))
            .Where(obj => obj.Domain != "eu");

        foreach (var (country, domain) in domains)
        {
            values.TryAdd(country, domain);
        }

        return values;
    }
}