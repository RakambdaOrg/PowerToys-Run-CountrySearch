using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using PowerToys_Run_CountrySearch_Generator.extractor;
using PowerToys_Run_CountrySearch_Generator.extractor.mass;
using PowerToys_Run_CountrySearch_Generator.extractor.single;
using PowerToys_Run_CountrySearch;
using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator;

public static class MainGenerator
{
    private static readonly IMassExtractor[] MassExtractors =
    [
        new FlagImageMassExtractor(),
        new DomainMassExtractor(),
        new DrivingSideMassExtractor(),
        new PhoneCodeMassExtractor(),
        new RegionMassExtractor()
    ];

    private static readonly ISingleExtractor[] SingleExtractors =
    [
        new MainLanguageSingleExtractor(),
        new OtherLanguageSingleExtractor(),
        new IconSingleExtractor()
    ];

    public static void Main()
    {
        var countriesJsonPath = Path.Join("..", "..", "..", "..", "Community.PowerToys.Run.Plugin.CountrySearch", "Resources", "countries.json");
        var countries = CountriesReader.ReadFile(countriesJsonPath);

        foreach (var extractor in MassExtractors)
        {
            var extracted = extractor.Extract();
            foreach (var (countryName, value) in extracted)
            {
                var country = countries.countries.Find(c => (c.name ?? "").Equals(countryName, StringComparison.InvariantCultureIgnoreCase));
                if (country == null)
                {
                    if (!extractor.CreateNewCountry())
                    {
                        continue;
                    }

                    country = new Country();
                    countries.countries.Add(country);
                    country.name = countryName;
                }

                InitEmptyObjects(country);
                SetValue(country, value, extractor.GetJsonPath(), extractor.OverrideIfSet());
            }
        }

        foreach (var extractor in SingleExtractors)
        {
            foreach (var country in countries.countries)
            {
                var value = extractor.Extract(country);
                SetValue(country, value, extractor.GetJsonPath(), extractor.OverrideIfSet());
            }
        }

        var jsonWriteOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var jsonContent = JsonSerializer.Serialize(countries, jsonWriteOptions);
        File.WriteAllText(countriesJsonPath, jsonContent);
    }

    private static void SetValue(object? obj, object? value, string[] jsonPath, bool overrideIfSet)
    {
        switch (value)
        {
            case null:
            case "":
                return;
        }

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

    private static void InitEmptyObjects(Country country)
    {
        country.flag ??= new Flag();
        country.flag.colors ??= [];
        country.flag.features ??= [];
        country.phone ??= new Phone();
        country.road ??= new Road();
        country.language ??= new Language();
        country.language.other ??= [];
    }
}