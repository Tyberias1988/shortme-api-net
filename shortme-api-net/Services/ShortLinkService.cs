using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shortme_api_net.Models;

namespace shortme_api_net.Services;

public interface IShortLinkService
{
    Task<List<ShortLink>> GetAllShortLinks();
    ShortLink Create(ShortLink shortLink);
    Task<bool> LinkInDB(string orgURL);
    Task<List<ShortLink>> GetSingleLink(string orgURL);
    int GetCountFromDB();
}

public class ShortLinkService : IShortLinkService
{

    private ApplicationDbContext _applicationDbContext;

    public ShortLinkService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<List<ShortLink>> GetAllShortLinks()
    {
        var links = await _applicationDbContext.ShortLinks.ToListAsync();
        return links;
    }

    public Task<bool> LinkInDB(string orgURL)
    {
        if (_applicationDbContext.ShortLinks.Any(l => l.OriginalUrl == orgURL))
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public async Task<List<ShortLink>> GetSingleLink(string orgURL)
    {
        var link = await _applicationDbContext.ShortLinks.Where(l => l.OriginalUrl == orgURL || l.Code == orgURL).ToListAsync();
        return link;
    }

    public int GetCountFromDB()
    {
        var count = _applicationDbContext.ShortLinks.Count();
        return count;
    }

    public ShortLink Create(ShortLink newShortLink)
    {
        _applicationDbContext.ShortLinks.Add(newShortLink);
        _applicationDbContext.SaveChanges();
        
        return newShortLink; 
    }
}