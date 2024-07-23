using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AAA_TEST_Console
{
    public class DBEntity
    {
        public string Field1 { get; set; }
    }

    public class FirstDbContext : IdentityDbContext
    {
        public FirstDbContext(DbContextOptions<FirstDbContext> options) : base(options) { }
        public DbSet<DBEntity> SomeDBEntities { get; set; } = null!;
    }

    internal class Database
    {
        internal void CreateDbIfNotExists()
        {
            var context = new FirstDbContext(null);

            context.Database.EnsureCreated();

            if (context.SomeDBEntities.Any())
            {
                return; // DB has been seeded already.
            }

            foreach (DBEntity en in new DBEntity[] { new() { Field1 = "aaa aaa aaa ..." } })
            {
                context.SomeDBEntities.Add(en);
            }

            context.SaveChanges();
        }
    }
}
