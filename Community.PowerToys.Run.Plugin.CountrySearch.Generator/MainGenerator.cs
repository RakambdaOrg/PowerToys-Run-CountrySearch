#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using PowerToys_Run_CountrySearch_Generator.extractor;
using PowerToys_Run_CountrySearch;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator;

public static class MainGenerator
{
    private static readonly IExtractor[] Extractors =
    [
        new FlagImageExtractor(),
        new DomainImageExtractor(),
        new DrivingSideExtractor(),
        new PhoneCodeExtractor(),
        new RegionExtractor(),
    ];

    public static void Main()
    {
        var countriesJsonPath = Path.Join("..", "..", "..", "..", "Community.PowerToys.Run.Plugin.CountrySearch", "Resources", "countries.json");
        var countries = CountriesReader.ReadFile(countriesJsonPath);

        foreach (var extractor in Extractors)
        {
            var extracted = extractor.Extract();
            foreach (var (countryName, value) in extracted)
            {
                var country = countries.countries.Find(c => c.name.Equals(countryName, StringComparison.InvariantCultureIgnoreCase));
                if (country == null)
                {
                    country = CreateNewCountry(countryName);
                    countries.countries.Add(country);
                }
                
                SetValue(country, value, extractor.GetJsonPath(), extractor.OverrideIfSet());
            }
        }

        var jsonWriteOptions = new JsonSerializerOptions { WriteIndented = true };
        var jsonContent = JsonSerializer.Serialize(countries, jsonWriteOptions);
        File.WriteAllText(countriesJsonPath, jsonContent);
    }

    private static string? GetCountryCodeFromName(string name)
    {
        return CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(c => new RegionInfo(c.Name))
            .FirstOrDefault(r => r.EnglishName == name)
            ?.TwoLetterISORegionName.ToLowerInvariant();
    }

    private static void SetValue(object? obj, string value, string[] jsonPath, bool overrideIfSet)
    {
        if (obj == null)
        {
            throw new Exception("Invalid JSON path, property with null value");
        }

        if (jsonPath.Length == 0)
        {
            throw new Exception("Invalid JSON path, missing parts");
        }

        var property = obj.GetType().GetProperty(jsonPath[0]);
        if (property == null)
        {
            throw new Exception("Invalid JSON path, property not found");
        }

        if (jsonPath.Length == 1)
        {
            if (!overrideIfSet)
            {
                var currentValue = property.GetValue(obj);
                if (typeof(string).IsAssignableFrom(property.PropertyType) && !string.IsNullOrWhiteSpace((string?)currentValue))
                {
                    return;
                }
            }

            property.SetValue(obj, value);
            return;
        }

        SetValue(property.GetValue(obj), value, jsonPath[1..], overrideIfSet);
    }

    private static Country CreateNewCountry(string name)
    {
        var country = new Country
        {
            domain = "",
            flag = new Flag
            {
                colors = [],
                features = [],
                file = ""
            },
            icon = "",
            name = name,
            phone = new Phone
            {
                code = ""
            },
            region = "",
            road = new Road
            {
                side = ""
            }
        };

        var isoCode = GetCountryCodeFromName(name);
        if (isoCode != null)
        {
            country.flag.file = $"{isoCode}.png";
        }

        return country;
    }
}