using Microsoft.EntityFrameworkCore;

namespace shortme_api_net.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<ShortLink> ShortLinks
    {
        get;
        set;
    }

}