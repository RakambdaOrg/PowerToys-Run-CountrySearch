using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
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
        new CountryMassExtractor(),
        new PhoneCodeMassExtractor(),
    ];

    private static readonly ISingleExtractor[] SingleExtractors =
    [
        new Cca2SingleExtractor(),
        new ContinentSingleExtractor(),
        new FlagFileSingleExtractor(),
        new LanguageSingleExtractor(),
        new RegionSingleExtractor(),
        new RoadSideSingleExtractor(),
        new TldSingleExtractor()
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
                var country = countries.Countries.Find(c => (c.Name ?? "").Equals(countryName, StringComparison.InvariantCultureIgnoreCase));
                if (country == null)
                {
                    if (!extractor.CreateNewCountry())
                    {
                        continue;
                    }

                    country = new Country();
                    countries.Countries.Add(country);
                    country.Name = countryName;
                }

                InitEmptyObjects(country);
                SetValue(country, value, extractor.GetPropertyPath(), extractor.OverrideIfSet());
            }
        }

        foreach (var extractor in SingleExtractors)
        {
            foreach (var country in countries.Countries)
            {
                var value = extractor.Extract(country);
                SetValue(country, value, extractor.GetPropertyPath(), extractor.OverrideIfSet());
            }
        }

        var jsonContent = JsonSerializer.Serialize(countries, CountriesReader.JsonSerializerOptions);
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
            return;
        }

        var property = obj.GetType().GetProperty(jsonPath[0]);
        if (property == null)
        {
            throw new Exception("Invalid JSON path, property not found");
        }

        if (jsonPath.Length == 1)
        {
            SetPropertyValue(property, obj, value, overrideIfSet);
            return;
        }

        SetValue(property.GetValue(obj), value, jsonPath[1..], overrideIfSet);
    }

    private static void SetPropertyValue(PropertyInfo property, object obj, object value, bool overrideIfSet)
    {
        if (ShouldNotSetValue(property, obj, overrideIfSet))
        {
            return;
        }

        property.SetValue(obj, value);
    }

    private static bool ShouldNotSetValue(PropertyInfo property, object obj, bool overrideIfSet)
    {
        if (overrideIfSet)
        {
            return false;
        }

        var currentValue = property.GetValue(obj);
        return currentValue switch
        {
            null => false,
            string strValue => !string.IsNullOrWhiteSpace(strValue),
            Array arrayValue => arrayValue.Length > 0,
            List<object> listValue => listValue.Count > 0,
            _ => false
        };
    }

    private static void InitEmptyObjects(Country country)
    {
        country.Flag ??= new Flag();
        country.Flag.Colors ??= [];
        country.Flag.Features ??= [];
        country.Tlds ??= [];
        country.Phone ??= new Phone();
        country.Road ??= new Road();
        country.Languages ??= [];
        country.Continents ??= [];
    }
}