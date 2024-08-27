using DotNet8.DbLogger;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Error> Errors { get; set; } = null!;
    }
}
