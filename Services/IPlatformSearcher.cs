namespace WebServiceAd.Services;

public interface IPlatformSearcher
{
    public IReadOnlyCollection<string> UploadPlatformRoutes(string source);

    public IReadOnlyCollection<string> GetPlatformsByRoute(string value);
}