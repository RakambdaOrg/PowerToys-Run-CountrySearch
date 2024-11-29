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
        private bool Disposed { get; set; }

        public List<Result> Query(Query query)
        {
            return
            [
                new Result
                {
                    QueryTextDisplay = query.Search,
                    IcoPath = IconPath,
                    Title = "MyTitle",
                    SubTitle = "MySubTitle",
                    ToolTipData = new ToolTipData("A tooltip title", "A tooltip text\nthat can have\nmultiple lines"),
                    Score = 1,
                }
            ];
        }

        public void Init(PluginInitContext context)
        {
            Log.Info("Init", GetType());
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
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

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? Context?.CurrentPluginMetadata.IcoPathLight : Context?.CurrentPluginMetadata.IcoPathDark;

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);
    }
}