using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;
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

    [HttpGet]
    [Route("{longUrl}")]
    public async Task<RedirectResult> ShortenUrl(string longUrl)
    {
        bool result = IsUrlValid(longUrl);

        if (!result)
        {
            return null;
        }

        string longUrlForCreation = CheckForURLScheme(longUrl);

        var shortenedUrlCollection = await _shortLinkService.GetAllShortLinks();

        foreach (var Item in shortenedUrlCollection)
        {
            if (Item.OriginalUrl.Contains(longUrl))
            {
                return Redirect(longUrlForCreation);
            }
        }

        var options = new GenerationOptions(useNumbers: true, useSpecialCharacters: false, length: 8);
        var shortCode = ShortId.Generate(options);
        DateTime now = DateTime.Now;
        ShortLink shortenedUrl = new ShortLink
        {
            Id = (shortenedUrlCollection.Count + 1),
            Code = shortCode,
            OriginalUrl = longUrlForCreation,
            CreatedAt = now,
            UpdatedAt = now
        };

        _shortLinkService.Create(shortenedUrl);

        return Redirect(longUrlForCreation);
    }

    private bool IsUrlValid(string url)
    {
        string regExPattern = @"^(http|https|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
        Regex reg = new Regex(regExPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return reg.IsMatch(url);
    }

    private string CheckForURLScheme(string url)
    {
        if (!Regex.IsMatch(url, @"^https|http?:\/\/", RegexOptions.IgnoreCase))
        {
            return url = "http://" + url;
        }

        return url;
    }
}