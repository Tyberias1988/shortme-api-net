using Microsoft.EntityFrameworkCore;
using shortme_api_net.Models;

namespace shortme_api_net.Services;

public interface IShortLinkService
{
    Task<List<ShortLink>> GetAllShortLinks();
    ShortLink Create(ShortLink shortLink);
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
        //var links = await _applicationDbContext.ShortLinks.Where(s => s.CreatedAt > DateTime.Now).ToListAsync();
        var links = await _applicationDbContext.ShortLinks.ToListAsync();
        return links;
    }

    public ShortLink Create(ShortLink newShortLink)
    {
        _applicationDbContext.ShortLinks.Add(newShortLink);
        _applicationDbContext.SaveChanges();
        
        return newShortLink; 
    }
}