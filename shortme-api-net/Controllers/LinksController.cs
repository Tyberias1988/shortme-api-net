using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shortme_api_net.Models;
using shortme_api_net.Services;
using shortme_api_net.Helpers;

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
    [Route("~/{shortID}")]
    public async Task<IActionResult> DoRedirectByShortID(string shortID)
    {
        var shortenedUrlCollection = await _shortLinkService.GetSingleLink(shortID);

        if(shortenedUrlCollection.Count == 0)
        {
            return NotFound();
        }

        string orgURL = shortenedUrlCollection[0].OriginalUrl;

        return Ok(orgURL);
    }

    [HttpPut("{longUrl}")]
    public async Task<List<ShortLink>> ShortenUrl(string longUrl)
    {
        List<ShortLink> sLink = new();

        bool result = IsUrlValid(longUrl);

        if (!result)
        {
            return sLink;
        }

        string longUrlForCreation = CheckForURLScheme(longUrl);

        bool linkIsInDB = await _shortLinkService.LinkInDB(longUrlForCreation);

        if (linkIsInDB)
        {
            var link = await _shortLinkService.GetSingleLink(longUrlForCreation);

            return link;
        }

        ShortLink shortenedUrl = CreateLinkObject(longUrlForCreation);

        sLink.Add(shortenedUrl);
        _shortLinkService.Create(shortenedUrl);

        return sLink;
    }

    private ShortLink CreateLinkObject(string longUrlForCreation)
    {
        string shortCode = RandomStringGenerator.RandomString(8);
        DateTime now = DateTime.Now;
        int count = _shortLinkService.GetCountFromDB();
        ShortLink shortenedUrl = new ShortLink
        {
            Id = (count + 1),
            Code = shortCode,
            OriginalUrl = longUrlForCreation,
            CreatedAt = now,
            UpdatedAt = now
        };

        return shortenedUrl;
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