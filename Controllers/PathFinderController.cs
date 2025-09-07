using Microsoft.AspNetCore.Mvc;
using WebServiceAd.Services;

namespace WebServiceAd.Controllers;

public class PathFinderController : BaseController
{
    private readonly IPlatformSearcher _platformSearcherService;


    public PathFinderController(IPlatformSearcher platformSearcherService)
    {
        _platformSearcherService = platformSearcherService;
    }


    [HttpPost("upload")]
    public async Task<ActionResult> UploadPlatforms([FromForm] IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("File contains no data");
        }

        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var data = await reader.ReadToEndAsync();

        var uploadedPlatforms = _platformSearcherService.UploadPlatformRoutes(data);
        var formattedPlatforms = String.Join('\n', uploadedPlatforms);

        return Ok(formattedPlatforms);
    }

    [HttpGet("search")]
    public ActionResult GetPlatformsByRoute(PlatformRoute route)
    {
        if (string.IsNullOrWhiteSpace(route.Route))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(route));
        }

        var platforms = _platformSearcherService.GetPlatformsByRoute(route.Route);
        if (platforms.Count == 0)
        {
            return NoContent();
        }

        return Ok(String.Join('\n', platforms));
    }


    public class PlatformRoute
    {
        public string Route { get; set; }
    }
}