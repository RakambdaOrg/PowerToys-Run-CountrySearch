using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using PowerToys_Run_CountrySearch;

namespace PowerToys_Run_CountrySearch_Generator.extractor;

public class FlagImageExtractor : IExtractor
{
    public string[] GetJsonPath()
    {
        return ["icon"];
    }

    public Dictionary<string, string> Extract()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://geohints.com/Flags");

        var bollardClasses = doc.DocumentNode.SelectNodes("//*[contains(@class,'bollard')]");
        foreach (var bollardClass in bollardClasses)
        {
            var country = bollardClass.SelectSingleNode("./*[contains(@class,'country')]").InnerText.CleanCountry();
            var flagPath = bollardClass.SelectSingleNode("./img").GetAttributeValue("src", null);
            if (string.IsNullOrEmpty(flagPath) || string.IsNullOrEmpty(country))
            {
                throw new Exception("Failed to extract flag image information");
            }

            values.TryAdd(country, $"https://geohints.com/{flagPath}");
        }

        return values;
    }
}