using System.IO;
using System.Text.Json;
using ManagedCommon;
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

    private Countries? Countries { get; set; }
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
                    IcoPath = Path.Join(PluginPath, "Images", "Flags", country.flag.file),
                    Title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(country.name),
                    SubTitle = country.Describe(),
                    Score = 1,
                    Action = _ => true
                }
            )
            .ToList();
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
        var jsonString = File.ReadAllText(countriesJsonPath);
        Countries = JsonSerializer.Deserialize<Countries>(jsonString) ?? new Countries { countries = [] };

        Database = new Dictionary<string, IEnumerable<Country>>();
        foreach (var country in Countries.countries.Where(country => !country.name.Equals("")))
        {
            AppendToKey(Database, country.name, country);
            AppendToKey(Database, country.region, country);
            AppendToKey(Database, country.domain, country);
            AppendToKey(Database, country.phone.code, country);
            AppendToKey(Database, country.road.side, country);
            country.flag.colors.ForEach(val => AppendToKey(Database, val, country));
            country.flag.features.ForEach(val => AppendToKey(Database, val, country));
        }
    }

    private static void AppendToKey(Dictionary<string, IEnumerable<Country>> dictionary, string key, Country value)
    {
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

    protected virtual void Dispose(bool disposing)
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