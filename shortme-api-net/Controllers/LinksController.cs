using Microsoft.AspNetCore.Mvc;
using shortme_api_net.Models;
using shortme_api_net.Services;

namespace shortme_api_net.Controllers;

[ApiController]
[Route("[controller]")]
public class LinksController : ControllerBase
{
    private readonly IShortLinkService _shortLinkService;

    public LinksController(IShortLinkService shortLinkService)
    {
        _shortLinkService = shortLinkService;
    }

    [HttpGet]
    public async Task<List<ShortLink>> GetAllShortLinks()
    {
        var links = await _shortLinkService.GetAllShortLinks();

        return links;
    }

}