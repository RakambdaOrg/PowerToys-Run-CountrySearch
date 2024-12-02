﻿using System.IO;
using System.Text.Json;
using ManagedCommon;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace PowerToys_Run_GeoGuessr
{
    public class Main : IPlugin, IDisposable
    {
        public static string PluginID => "77F3BA9738C64452B128F24E1B7E7B44";
        public string Name => "GeoGuessr";
        public string Description => "Be a good cheater";
        private PluginInitContext? Context { get; set; }
        private string? IconPath { get; set; }
        private Countries countries { get; set; }
        private Dictionary<string, IEnumerable<Country>> database { get; set; }
        private bool Disposed { get; set; }

        public List<Result> Query(Query query)
        {
            return query.Terms
                .Where(term => database.ContainsKey(term))
                .Select(term => database[term])
                .Aggregate(new List<Country>() as IEnumerable<Country>, (a, b) => a.Intersect(b))
                .Select(country => new Result
                    {
                        QueryTextDisplay = query.Search,
                        IcoPath = country.icon,
                        Title = country.name,
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

            var jsonString = File.ReadAllText("Resources\\countries.json");
            countries = JsonSerializer.Deserialize<Countries>(jsonString) ?? new Countries() { countries = [] };

            database = new Dictionary<string, IEnumerable<Country>>();
            foreach (var country in countries.countries.Where(country => !country.name.Equals("")))
            {
                AppendToKey(database, country.name, country);
                AppendToKey(database, country.region, country);
                AppendToKey(database, country.domain, country);
                AppendToKey(database, country.road.side, country);
                country.flag.colors.ForEach(val => AppendToKey(database, val, country));
                country.flag.features.ForEach(val => AppendToKey(database, val, country));
            }
        }

        private static void AppendToKey<TK, TV>(Dictionary<TK, IEnumerable<TV>> dictionary, TK key, TV value) where TK : notnull
        {
            if (!dictionary.TryGetValue(key, out var values))
            {
                values = new List<TV>();
                dictionary.Add(key, values);
            }

            dictionary[key] = values.Append(value);
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
}