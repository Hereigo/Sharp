using Calendarium.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Calendarium.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CalEvent> CalEvents { get; set; } = null!;

        public DbSet<CalEventCategory> CalEventCategories { get; set; } = null!;

        public DbSet<RequestHeaderField> RequestsHeaders { get; set; } = null!;

        public DbSet<Note> Notes { get; set; }
    }
}
 