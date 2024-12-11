namespace PowerToys_Run_CountrySearch;

public static class Extensions
{
    public static string? Transform(this string source, Func<string, string> transform, string? fallback = null)
    {
        return string.IsNullOrWhiteSpace(source) ? fallback : transform(source);
    }

    public static string Cleanup(this string source)
    {
        return source
            .Replace("Ã§", "ç")
            .Replace("&ccedil;", "ç")
            .Replace("&#160;", " ")
            ;
    }
}