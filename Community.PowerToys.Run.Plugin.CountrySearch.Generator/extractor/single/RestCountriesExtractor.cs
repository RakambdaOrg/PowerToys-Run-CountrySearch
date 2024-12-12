using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public abstract class RestCountriesExtractor
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    private static readonly CountryInfoResponse[]? CountryInfo = CountryInfoGenerator();

    protected static CountryInfoResponse? GetCountryInfoByCca2(string code)
    {
        return CountryInfo?.FirstOrDefault(c => c.Cca2 == code);
    }

    protected static CountryInfoResponse? GetCountryInfoByName(string name)
    {
        return CountryInfo?.FirstOrDefault(c => c.Name?.Common == name || c.Name?.Official == name);
    }

    protected static CountryInfoResponse[]? CountryInfoGenerator()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(new Uri("https://restcountries.com"), "/v3.1/all?fields=tld,languages,cca2,region,car,name,continents")
        };

        var response = client.Send(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        using var stream = response.Content.ReadAsStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = reader.ReadToEnd();
        return JsonSerializer.Deserialize<CountryInfoResponse[]>(content, JsonSerializerOptions);
    }

    protected class CountryInfoResponse
    {
        public Dictionary<string, string>? Languages { get; set; }
        public string[]? Tld { get; set; }
        public string[]? Continents { get; set; }
        public string? Cca2 { get; set; }
        public string? Region { get; set; }
        public Car? Car { get; set; }
        public Name? Name { get; set; }
    }

    protected class Car
    {
        public string? Side { get; set; }
    }

    protected class Name
    {
        public string? Common { get; set; }
        public string? Official { get; set; }
    }
}