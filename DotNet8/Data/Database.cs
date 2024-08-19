using DotNet8.Models;

namespace DotNet8.Data
{
    internal static class Database
    {
        internal static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    context.Database.EnsureCreated();

                    if (context.CalEvents.Any())
                    {
                        return; // DB has been seeded already.
                    }

                    var dtaToday = DateTime.Now;
                    var defaultCategory = new CalEventCategory { Name = "default" };

                    context.CalEventCategories.Add(defaultCategory);
                    context.SaveChanges();

                    foreach (CalEvent evt in new CalEvent[] {
                        new("Test aaa aaa ...", defaultCategory){},
                    })
                    {
                        context.CalEvents.Add(evt);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
