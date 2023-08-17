using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Blockchain.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}