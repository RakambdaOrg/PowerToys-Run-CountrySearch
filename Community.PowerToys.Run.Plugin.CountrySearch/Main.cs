﻿using System.IO;
using ManagedCommon;
using PowerToys_Run_CountrySearch.model;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace PowerToys_Run_CountrySearch;

public class Main : IPlugin, IDisposable
{
    public static string PluginID => "77F3BA9738C64452B128F24E1B7E7B44";
    public string Name => "CountrySearch";
    public string Description => "Try to find a country based on its information";

    private PluginInitContext? Context { get; set; }
    private string? PluginPath { get; set; }
    private string? IconPath { get; set; }
    private bool Disposed { get; set; }

    private CountriesSchema? Countries { get; set; }
    private Dictionary<string, IEnumerable<Country>>? Database { get; set; }

    public List<Result> Query(Query query)
    {
        if (Database == null)
        {
            return [];
        }

        var matchingTermCountry = query.Terms
            .Select(term => term.ToLowerInvariant())
            .Where(term => Database!.ContainsKey(term))
            .Select(term => Database![term])
            .ToList();

        var matchingAllTerms = matchingTermCountry.Count <= 1
            // If we have 0 or 1 term, return them as is
            ? matchingTermCountry.SelectMany((source, _) => source)
            // If we have more than 2 terms, take the intersection of the results
            : matchingTermCountry.Aggregate((a, b) => a.Intersect(b));

        return matchingAllTerms
            .Select(country => new Result
                {
                    QueryTextDisplay = query.Search,
                    IcoPath = country.Flag?.File?.Transform(p => Path.Join(PluginPath, "Images", "Flags", p)),
                    Title = country.Name,
                    SubTitle = Describe(country),
                    Score = 1,
                    Action = _ => true
                }
            )
            .ToList();
    }

    private static string Describe(Country c)
    {
        return $"""
                Continent: {c.Continents?.FirstOrDefault()} | Region: {c.Region} | Domain: {c.Tlds?.FirstOrDefault()}
                Road side: {c.Road?.Side} | Language: {c.Languages?.FirstOrDefault()?.Name} | Phone code: +{c.Phone?.Code}
                """;
    }

    public void Init(PluginInitContext context)
    {
        Log.Info("Init", GetType());
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Context.API.ThemeChanged += OnThemeChanged;
        UpdateIconPath(Context.API.GetCurrentTheme());

        InitPluginPath(Context.CurrentPluginMetadata.PluginDirectory);
        InitDatabase(Path.Join(Context.CurrentPluginMetadata.PluginDirectory, "Resources", "countries.json"));
    }

    public void InitPluginPath(string path)
    {
        PluginPath = path;
    }

    public void InitDatabase(string countriesJsonPath)
    {
        Countries = CountriesReader.ReadFile(countriesJsonPath);

        Database = new Dictionary<string, IEnumerable<Country>>();
        foreach (var country in Countries.Countries.Where(country => !string.IsNullOrEmpty(country.Name)))
        {
            AppendToKey(Database, country.Name, country);
            AppendToKey(Database, country.Region, country);
            AppendToKey(Database, country.Phone?.Code, country);
            AppendToKey(Database, country.Road?.Side, country);
            country.Tlds?.ForEach(val => AppendToKey(Database, val, country));
            country.Languages?.ForEach(val => AppendToKey(Database, val.Name, country));
            country.Continents?.ForEach(val => AppendToKey(Database, val, country));
            country.Flag?.Colors?.ForEach(val => AppendToKey(Database, val, country));
            country.Flag?.Features?.ForEach(val => AppendToKey(Database, val, country));
        }
    }

    private static void AppendToKey(Dictionary<string, IEnumerable<Country>> dictionary, string? key, Country value)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (!dictionary.TryGetValue(key, out var values))
        {
            values = new List<Country>();
            dictionary.Add(key, values);
        }

        dictionary[key.ToLowerInvariant()] = values.Append(value);
    }

    public void Dispose()
    {
        Log.Info("Dispose", GetType());
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (Disposed || !disposing)
        {
            return;
        }

        if (Context?.API != null)
        {
            Context.API.ThemeChanged -= OnThemeChanged;
        }

        Disposed = true;
    }

    private void UpdateIconPath(Theme theme) => IconPath = theme is Theme.Light or Theme.HighContrastWhite ? Context?.CurrentPluginMetadata.IcoPathLight : Context?.CurrentPluginMetadata.IcoPathDark;

    private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);
}