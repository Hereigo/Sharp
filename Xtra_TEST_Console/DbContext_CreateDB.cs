using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AAA_TEST_Console
{
    public class DBEntity
    {
        public string Field1 { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<DBEntity> CalEvents { get; set; } = null!;
    }

    internal class Database
    {
        internal void CreateDbIfNotExists()
        {
            var context = new ApplicationDbContext(null);

            context.Database.EnsureCreated();

            if (context.CalEvents.Any())
            {
                return; // DB has been seeded already.
            }

            foreach (DBEntity en in new DBEntity[] { new() { Field1 = "aaa aaa aaa ..." } })
            {
                context.CalEvents.Add(en);
            }

            context.SaveChanges();
        }
    }
}
