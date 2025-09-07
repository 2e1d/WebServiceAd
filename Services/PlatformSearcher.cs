using System.Text.RegularExpressions;
using WebServiceAd.Extensions;
using WebServiceAd.Models.Tree;

namespace WebServiceAd.Services;

public partial class PlatformSearcher : IPlatformSearcher
{
    private Tree _locationTree;

    private static readonly Regex PlatformDataValidatorRegex = GeneratePlatformDataRegex();

    public IReadOnlyCollection<string> UploadPlatformRoutes(string source)
    {
        var lines = source.SplitNotEmpty('\n');
        var matches = lines
            .Select(l => PlatformDataValidatorRegex.Match(l))
            .Where(m => m.Length > 0 && m.Groups.Count > 0)
            .ToArray();
        if (matches.Length == 0)
        {
            throw new ArgumentException("Invalid data format", nameof(source));
        }

        _locationTree = new Tree();
        var uploadedPlatforms = new List<string>();
        foreach (var match in matches)
        {
            var matchGroupValues = match.Groups.Values.ToArray();
            var platform = matchGroupValues[1].Value;
            var routes = matchGroupValues
                .Skip(2)
                .Where(m => m.Value.Length > 0)
                .Select(m => m.Value)
                .ToArray();

            _locationTree.Add(platform, routes);
            uploadedPlatforms.Add(platform);
        }

        return uploadedPlatforms;
    }

    public IReadOnlyCollection<string> GetPlatformsByRoute(string value)
    {
        if (_locationTree == null)
        {
            throw new ApplicationException("GetPlatforms must be called first");
        }

        var resultPlatforms = new HashSet<string>();
        var currentNode = _locationTree.Root;

        var routes = value.SplitNotEmpty('/');
        foreach (var location in routes)
        {
            if (!_locationTree.IsLocationPresented(location, currentNode))
            {
                break;
            }

            var foundNode = currentNode.Children.First(x => x.Location == location);
            foreach (var platform in foundNode.Platforms)
            {
                resultPlatforms.Add(platform);
            }

            currentNode = foundNode;
        }

        return resultPlatforms;
    }

    [GeneratedRegex(@"^([^:\/]+):(\/[^,\/]+(?:\/[^,\/]*)*)(?:,(\/[^,\/]+(?:\/[^,\/]*)*))*$", RegexOptions.Compiled)]
    private static partial Regex GeneratePlatformDataRegex();
}