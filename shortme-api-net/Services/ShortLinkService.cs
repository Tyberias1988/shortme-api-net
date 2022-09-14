using Microsoft.EntityFrameworkCore;
using shortme_api_net.Models;

namespace shortme_api_net.Services;

public interface IShortLinkService
{
    Task<List<ShortLink>> GetAllShortLinks();
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
        var links = await _applicationDbContext.ShortLinks.Where(s => s.CreatedAt > DateTime.Now).ToListAsync();
        return links;
    }
    
}