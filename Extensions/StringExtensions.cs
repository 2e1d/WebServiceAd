namespace WebServiceAd.Extensions;

public static class StringExtensions
{
    public static string[] SplitNotEmpty(this string source, char separator)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }
}