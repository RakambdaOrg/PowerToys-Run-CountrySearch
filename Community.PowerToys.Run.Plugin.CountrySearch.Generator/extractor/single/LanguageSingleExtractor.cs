using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PowerToys_Run_CountrySearch_Generator.extractor.single;

public abstract class LanguageSingleExtractor
{
    private static Dictionary<string, CountryInfoResponse>? _countriesInfo;

    protected static CountryInfoResponse? GetCountryInfo(string code)
    {
        if (_countriesInfo == null)
        {
            _countriesInfo = new Dictionary<string, CountryInfoResponse>();

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(new Uri("https://restcountries.com"), $"/v3.1/all?fields=tld,languages")
            };

            var response = client.Send(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using var stream = response.Content.ReadAsStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var content = reader.ReadToEnd();
            var responseData = JsonSerializer.Deserialize<CountryInfoResponse[]>(content);
            if (responseData == null)
            {
                return null;
            }

            foreach (var infoResponse in responseData)
            {
                foreach (var tld in infoResponse.tld)
                {
                    _countriesInfo.TryAdd(tld[1..], infoResponse);
                }
            }
        }

        _countriesInfo.TryGetValue(code, out var info);
        return info;
    }

    protected class CountryInfoResponse(
        Dictionary<string, string> languages,
        string[] tld
    )
    {
        public Dictionary<string, string> languages { get; init; } = languages;
        public string[] tld { get; init; } = tld;
    }
}